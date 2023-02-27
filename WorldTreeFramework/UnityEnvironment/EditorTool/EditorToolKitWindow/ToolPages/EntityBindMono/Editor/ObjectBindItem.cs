
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/30 17:34

* 描述： 对象绑定项

*/

using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using UnityEngine.UI;
using WorldTree;

namespace EditorTool
{
    [Serializable]
    public class ObjectBindItem
    {
        [HideInInspector]
        public bool IsShow = true;

        [HideInInspector]
        public bool IsRefresh = false;

        [GUIColor(0, 1, 1)]
        [ReadOnly]
        [HideLabel]
        [HorizontalGroup("A")]
        [HorizontalGroup("A/A",MinWidth = 250)]
        public MonoObject monoObject;

        [HideLabel]
        [HorizontalGroup("A/A" )]
        public string comment;


        [GUIColor(0, 1, 0)]
        [HorizontalGroup("A/A", width: 60)]
        [Button("生成脚本", ButtonSizes.Medium)]
        public void CreateScript()
        {
            CreateBindScript();
            CreateBindEventScript();
            CreateBindViewScript();
        }



        [ReadOnly]
        [HideLabel]
        [HorizontalGroup("A/B")]
        public UnityEngine.Object entityScript;

        [HideInInspector]
        public UnityEngine.Object entityViewScript;
        [HideInInspector]
        public UnityEngine.Object entityEventScript;

        [GUIColor(0, 1, 0)]
        [ShowIf("@!IsShow&&components.Count>0")]
        [HorizontalGroup("A", width: 100)]
        [Button("组件列表", ButtonSizes.Medium)]
        public void FoldShow()
        {
            IsShow = true;
        }
        [GUIColor(1, 1, 0)]
        [ShowIf("@IsShow&&components.Count>0")]
        [HorizontalGroup("A", width: 100)]
        [Button("组件列表", ButtonSizes.Medium)]
        public void FoldHide()
        {
            IsShow = false;
        }

        [ShowIf("@IsShow&&components.Count>0")]
        [LabelText("组件列表")]
        [Searchable]
        [ListDrawerSettings(IsReadOnly = true, Expanded = true)]
        public List<ComponentBindItem> components = new List<ComponentBindItem>();

        public void UpdateRefresh()
        {
            if (components.Count != monoObject.components.Count)
            {
                IsRefresh = true;
            }
            else
            {
                for (int i = 0; i < monoObject.components.Count; i++)
                {
                    if (components[i].component != monoObject.components[i])
                    {
                        IsRefresh = true;
                        break;
                    }
                }
            }
            if (IsRefresh)
            {
                IsRefresh = false;

                List<ComponentBindItem> newComponents = new List<ComponentBindItem>();

                for (int i = 0; i < monoObject.components.Count; i++)
                {
                    ComponentBindItem component = components.Find(x => x.component == monoObject.components[i]);
                    if (component != null)
                    {
                        newComponents.Add(component);
                    }
                    else
                    {
                        ComponentBindItem componentItem = new ComponentBindItem() { component = monoObject.components[i] };
                        newComponents.Add(componentItem);
                        componentItem.Refresh();
                    }
                }
                components = newComponents;
            }
        }

        public void DeleteBindScript()
        {
            string path = EntityBindMonoTool.Inst.CreateFilePath + $"/{monoObject.gameObject.name}";
            if (Directory.Exists(path))
            {
                AssetDatabase.DeleteAsset(path);
            }
        }


        public void CreateBindScript()
        {
            string path = EntityBindMonoTool.Inst.CreateFilePath + $"/{monoObject.gameObject.name}/" + monoObject.gameObject.name + ".cs";

            if (!entityScript)
            {
                StringBuilder builder = new StringBuilder();

                builder.AppendLine("namespace WorldTree");
                builder.AppendLine("{");
                builder.AppendLine($"\tpublic class {monoObject.gameObject.name} : Entity");
                builder.AppendLine("\t{");
                builder.AppendLine($"\t\tpublic {monoObject.gameObject.name}View view;");

                builder.AppendLine("\n\t\t#region 回调事件\n");

                foreach (var component in components)
                {
                    if (Script.EventStrings.TryGetValue(component.component.GetType(), out string[] EventStrings))
                    {
                        for (int i = 0; i < EventStrings.Length; i++)
                        {
                            if (component.eventTags[i].bit)
                            {
                                builder.AppendLine($"\t\tpublic void {Script.GetFieldName(component.component)}{EventStrings[i]}");
                                builder.AppendLine("\t\t{\n");
                                builder.AppendLine("\t\t}");
                            }
                        }
                    }
                }
                builder.AppendLine("\n\t\t#endregion");
                builder.AppendLine("\t}");
                builder.AppendLine("}");

                Directory.CreateDirectory(Path.GetDirectoryName(path));//如果文件夹不存在就创建它
                File.WriteAllText(path, builder.ToString());
                AssetDatabase.Refresh();
                entityScript = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                builder.Clear();
            }
            else
            {
                UIFormEditor.ShowWindow((entityScript as TextAsset).text, path, GetEventMethodString());
            }
        }


        public Dictionary<string, string> GetEventMethodString()
        {
            StringBuilder builder = new StringBuilder();

            Dictionary<string, string> keyValues = new Dictionary<string, string>();

            foreach (var component in components)
            {
                if (Script.EventStrings.TryGetValue(component.component.GetType(), out string[] EventStrings))
                {
                    for (int i = 0; i < EventStrings.Length; i++)
                    {
                        builder.Clear();
                        if (component.eventTags[i].bit)
                        {
                            string key = Script.GetFieldName(component.component) + EventStrings[i];
                            builder.AppendLine($"public void {key}");
                            builder.AppendLine("\t\t{\n");
                            builder.AppendLine("\t\t}");
                            keyValues.Add(key, builder.ToString());
                        }
                    }
                }
            }
            return keyValues;
        }




        public void CreateBindEventScript()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("namespace WorldTree");
            builder.AppendLine("{");
            builder.AppendLine($"\tclass {monoObject.gameObject.name}_AddEventSystem : AddSystem<{monoObject.gameObject.name}>");
            builder.AppendLine("\t{");
            builder.AppendLine($"\t\tpublic override void OnEvent({monoObject.gameObject.name} self)");
            builder.AppendLine("\t\t{");
            builder.AppendLine($"\t\t\tself.view = self.Parent.AddComponent<{monoObject.gameObject.name}View>();");


            foreach (var component in components)
            {
                if (Script.EventRegisters.TryGetValue(component.component.GetType(), out string[] registers))
                {
                    if (Script.EventNames.TryGetValue(component.component.GetType(), out string[] EventNames))
                    {
                        for (int i = 0; i < registers.Length; i++)
                        {
                            if (component.eventTags[i].bit)
                            {
                                string name = Script.GetFieldName(component.component);
                                builder.AppendLine($"\t\t\tself.view.{name}{string.Format(registers[i], $"self.{name}{EventNames[i]}")};");
                            }
                        }
                    }
                }
            }
            builder.AppendLine("\t\t}");
            builder.AppendLine("\t}");
            //builder.AppendLine($"\tclass {monoObject.gameObject.name}_RemoveEventSystem : RemoveSystem<{monoObject.gameObject.name}>");
            //builder.AppendLine("\t{");
            //builder.AppendLine($"\t\tpublic override void OnEvent({monoObject.gameObject.name} self)");
            //builder.AppendLine("\t\t{");
            //builder.AppendLine($"\t\t\tself.Parent.Dispose();");
            //builder.AppendLine("\t\t}");
            //builder.AppendLine("\t}");
            builder.AppendLine("}");

            string path = EntityBindMonoTool.Inst.CreateFilePath + $"/{monoObject.gameObject.name}/" + $"{monoObject.gameObject.name}_AddEventSystem.cs";
            Directory.CreateDirectory(Path.GetDirectoryName(path));//如果文件夹不存在就创建它
            File.WriteAllText(path, builder.ToString());
            AssetDatabase.Refresh();
            entityEventScript = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            builder.Clear();
        }


        public void CreateBindViewScript()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in Script.usings)
            {
                if (components.Any((component) => item.Value.Contains(component.component.GetType())))
                {
                    builder.AppendLine(item.Key);
                }
            }

            builder.AppendLine("namespace WorldTree");
            builder.AppendLine("{");

            builder.AppendLine($"\t/// <summary>");
            builder.AppendLine($"\t/// {comment}");
            builder.AppendLine($"\t/// </summary>");
            builder.AppendLine($"\tpublic class {monoObject.gameObject.name}View : Entity");
            builder.AppendLine("\t{");
            builder.AppendLine($"\t\t/// <summary>");
            builder.AppendLine($"\t\t/// Mono组件");
            builder.AppendLine($"\t\t/// </summary>");
            builder.AppendLine("\t\tpublic MonoObject monoObject;");
            foreach (var component in components)
            {
                builder.AppendLine($"\t\t/// <summary>");
                builder.AppendLine($"\t\t/// {component.comment}");
                builder.AppendLine($"\t\t/// </summary>");
                builder.AppendLine($"\t\t{Script.GetField(component.component)}");
            }
            builder.AppendLine("\t}");

            builder.AppendLine($"\tclass {monoObject.gameObject.name}ViewAddSystem : AddSystem<{monoObject.gameObject.name}View>");
            builder.AppendLine("\t{");
            builder.AppendLine($"\t\tpublic override void OnEvent({monoObject.gameObject.name}View self)");
            builder.AppendLine("\t\t{");
            builder.AppendLine("\t\t\tif (self.ParentTo<GameObjectEntity>().gameObject.TryGetComponent(out self.monoObject))");
            builder.AppendLine("\t\t\t{");
            for (int i = 0; i < components.Count; i++)
            {
                builder.AppendLine($"\t\t\t\t{Script.GetFieldBind(components[i].component, i)}");
            }
            builder.AppendLine("\t\t\t}");
            builder.AppendLine("\t\t}");
            builder.AppendLine("\t}");

            builder.AppendLine($"\tclass {monoObject.gameObject.name}ViewRemoveSystem : RemoveSystem<{monoObject.gameObject.name}View>");
            builder.AppendLine("\t{");
            builder.AppendLine($"\t\tpublic override void OnEvent({monoObject.gameObject.name}View self)");
            builder.AppendLine("\t\t{");
            builder.AppendLine("\t\t\tself.monoObject?.RemoveAllEvent();");
            builder.AppendLine("\t\t\tself.monoObject = null;");
            foreach (var component in components)
            {
                builder.AppendLine($"\t\t\tself.{Script.GetFieldName(component.component)} = null;");
            }
            builder.AppendLine("\t\t}");
            builder.AppendLine("\t}");


            builder.AppendLine("}");

            string path = EntityBindMonoTool.Inst.CreateFilePath + $"/{monoObject.gameObject.name}/" + monoObject.gameObject.name + "View.cs";
            Directory.CreateDirectory(Path.GetDirectoryName(path));//如果文件夹不存在就创建它
            File.WriteAllText(path, builder.ToString());
            AssetDatabase.Refresh();
            entityViewScript = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            builder.Clear();

        }


        /// <summary>
        /// 获取插入代码的下标
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public int GetInsertIndex(string content, string tag)
        {
            //找到 UI事件管理 下面的第一个 public 所在的位置 进行插入
            Regex regex = new Regex(tag);
            Match match = regex.Match(content);

            Regex regex1 = new Regex("public");
            MatchCollection matchCollection = regex1.Matches(content);

            for (int i = 0; i < matchCollection.Count; i++)
            {
                if (matchCollection[i].Index > match.Index)
                {
                    return matchCollection[i].Index;
                }
            }

            return -1;
        }

    }


    public static class Script
    {
        public static Dictionary<string, HashSet<Type>> usings = new Dictionary<string, HashSet<Type>>()
        {
            ["using UnityEngine;"] = new HashSet<Type>{
                typeof(CanvasGroup),
            },
            ["using UnityEngine.UI;"] = new HashSet<Type> {
                typeof(Image),
                typeof(RawImage),
                typeof(Button),
                typeof(Text),
                typeof(InputField),

                typeof(Toggle),
                typeof(ToggleGroup),

                typeof(ScrollRect),

            },

        };

        public static Dictionary<Type, string[]> EventNames = new Dictionary<Type, string[]>()
        {
            [typeof(InputField)] = new string[] { "OnValueChanged", "OnSubmit", "OnEndEdit" },
            [typeof(Toggle)] = new string[] { "onValueChanged" },
            [typeof(Button)] = new string[] { "OnClick" },
        };

        public static Dictionary<Type, string[]> EventStrings = new Dictionary<Type, string[]>()
        {
            [typeof(InputField)] = new string[] { "OnValueChanged(string str)", "OnSubmit(string str)", "OnEndEdit(string str)" },
            [typeof(Toggle)] = new string[] { "OnValueChanged(bool toggle)" },
            [typeof(Button)] = new string[] { "OnClick()" },
        };

        public static Dictionary<Type, string[]> EventRegisters = new Dictionary<Type, string[]>()
        {
            [typeof(InputField)] = new string[] { ".onValueChanged.AddListener({0})", ".onSubmit.AddListener({0})", ".onEndEdit.AddListener({0})" },
            [typeof(Toggle)] = new string[] { ".onValueChanged.AddListener({0})" },
            [typeof(Button)] = new string[] { ".onClick.AddListener({0})" },
        };


        public static string GetFieldName(Component component)
        {
            return component.name + component.GetType().Name;
        }

        public static string GetField(Component component)
        {
            return $"public {component.GetType().Name} {GetFieldName(component)};";
        }
        public static string GetFieldBind(Component component, int index)
        {
            return $"self.{component.name}{component.GetType().Name} = self.monoObject.components[{index}] as {component.GetType().Name};";
        }
    }

}
