
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/30 17:34

* 描述： 对象绑定项

*/

using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace WorldTree
{
    [Serializable]
    public class ObjectBindItem
    {
        [HideInInspector]
        public ObjectBindGroup objectBindGroup;

        [HideInInspector]
        public bool IsShow = true;

        [HideInInspector]
        public bool IsRefresh = false;

        [GUIColor(0, 1, 1)]
        [ReadOnly]
        [HideLabel]
        [HorizontalGroup("A", width: 400)]
        [HorizontalGroup("A/A", width: 300)]
        public MonoObject monoObject;


        [GUIColor(0, 1, 0)]
        [HorizontalGroup("A/A", width: 100)]
        [Button("生成脚本", ButtonSizes.Medium)]
        public void CreateScript()
        {
            CreateBindScript();
        }

        public void DeleteScript()
        {

        }

        [ReadOnly]
        [HideLabel]
        [HorizontalGroup("A/B")]
        public UnityEngine.Object entityScript;

        [GUIColor(0, 1, 0)]
        [ShowIf("@!IsShow&&components.Count>0")]
        [HorizontalGroup("A", width: 150)]
        [Button("组件列表", ButtonSizes.Medium)]
        public void FoldShow()
        {
            IsShow = true;
        }
        [GUIColor(1, 0, 0)]
        [ShowIf("@IsShow&&components.Count>0")]
        [HorizontalGroup("A", width: 150)]
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


        public void CreateBindScript()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("using System;");
            builder.AppendLine("using System.Collections.Generic;");

            builder.AppendLine("namespace WorldTree");
            builder.AppendLine("{");

            builder.AppendLine($"   public partial class {monoObject.gameObject.name} : Entity");
            builder.AppendLine("   {");

            foreach (var component in components)
            {

            }


            builder.AppendLine("   }");
            builder.AppendLine("}");



            string path = objectBindGroup.monoBindEntityTool.CreateFilePath + "/" + monoObject.gameObject.name + ".cs";
            File.WriteAllText(path, builder.ToString());
            AssetDatabase.Refresh();
            entityScript = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            builder.Clear();

        }
    }


    public static class CreateBindScript
    {


    }

}
