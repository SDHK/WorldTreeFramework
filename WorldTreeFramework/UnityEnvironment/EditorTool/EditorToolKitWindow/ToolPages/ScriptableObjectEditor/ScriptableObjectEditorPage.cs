using UnityEngine;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;

namespace WorldTree
{
    public enum EditorFieldAllType
    {
        String,
        Int,
        Float,
        Bool,

        Dictionary,
        List,

        GameObject,

        其它

    }

    public enum EditorFieldType
    {
        String,
        Int,
        Float,
        Bool,
        GameObject,
    }

    //简单类型集合
    //List页面只用于编辑，思考通用List页面，应该与这个Class列表关联，和脚本资源不关联
    //List页面或许能直接画出来,不需要创建Asset？？？？？？

    //资源应该在AB包里，而生成的脚本在热更里
    [CreateAssetMenu]
    public class ScriptableObjectEditorPage : ScriptableObject
    {
        [BoxGroup("路径")]
        [FolderPath(RequireExistingPath = true)]
        [LabelText("脚本文件夹"), LabelWidth(100)]
        public string CreateFilePath;
        [BoxGroup("路径")]
        [FolderPath(RequireExistingPath = true)]
        [LabelText("资源文件夹"), LabelWidth(100)]
        public string CreateAssetPath;

        //[InlineEditor]
        //public ScriptableObject guidePage;

        //[InlineEditor]
        //[ListDrawerSettings(CustomRemoveElementFunction = "Remove")]
        //public List<ScriptableObject> list = new List<ScriptableObject>();

        //public void Remove(ScriptableObject o)
        //{
        //    Debug.Log("!!!");
        //}



        [InfoBox("命名冲突", InfoMessageType.Error, "@this.IsRepeatName()")]
        [BoxGroup("编辑")]
        [LabelText("新建类名"), LabelWidth(60)]
        public string ClassName;

        [BoxGroup("编辑")]
        [Button("添加", ButtonSizes.Large)]
        public void AddClass()
        {
            if (!IsRepeatName() && ClassName != "")
            {
                classes.Add(new EditorAssetClass() { ClassName = ClassName, scriptableObjectEditor = this });
                ClassName = "";
            }
        }




        public bool IsRepeatName()
        {
            return classes.Any((item) => item.ClassName == ClassName);
        }

        [LabelText("列表")]
        [Searchable]
        [ListDrawerSettings(Expanded = true, HideAddButton = true, CustomRemoveElementFunction = "RemoveClass")]
        public List<EditorAssetClass> classes = new List<EditorAssetClass>();


        public void RemoveClass(EditorAssetClass assetClass)
        {
            assetClass.RemoveAllAsset();
            classes.Remove(assetClass);
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(assetClass.monoScript));
        }
    }

    [Serializable]
    public class EditorAssetClass
    {


        [HideInInspector]
        public ScriptableObjectEditorPage scriptableObjectEditor;

        [HorizontalGroup("A")]
        [HorizontalGroup("A/A")]
        [LabelText("类名"), LabelWidth(30), GUIColor(0, 1, 0)]
        [ReadOnly]
        public string ClassName;

        [HorizontalGroup("A/A")]
        [LabelText("注释"), LabelWidth(30)]
        public string Comment;

        [EnableIf("@this.IsCreateClass()")]

        //[EnableIf("@!System.String.IsNullOrEmpty(this.ClassName)&&!this.IsRepeatFieldName()&&!System.String.IsNullOrEmpty(scriptableObjectEditor.CreateFilePath)")]
        [HorizontalGroup("A/A")]
        [Button("生成脚本")]
        public void CreateClassFile()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("using System;");
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine("using UnityEngine;");

            builder.AppendLine($"namespace WorldTree");
            builder.AppendLine("{");
            builder.AppendLine("    /// <summary>");
            builder.AppendLine($"    /// {Comment}");
            builder.AppendLine("    /// </summary>");
            builder.AppendLine($"   public class {ClassName}:ScriptableObject");
            builder.AppendLine("    {");

            foreach (var field in fields)
            {
                builder.AppendLine("        /// <summary>");
                builder.AppendLine($"        /// {field.Comment}");
                builder.AppendLine("        /// </summary>");

                switch (field.FieldType)
                {
                    case EditorFieldAllType.Dictionary:
                        builder.AppendLine($"        public ListDictionary<{StringToTypeName(field.KeyType.ToString())},{StringToTypeName(field.ValueType.ToString())}> {field.FieldName};");
                        ; break;

                    case EditorFieldAllType.List:
                        builder.AppendLine($"        public List<{StringToTypeName(field.ItemType.ToString())}> {field.FieldName};");
                        ; break;

                    case EditorFieldAllType.其它:
                        builder.AppendLine($"        public {field.TypeName} {field.FieldName};");
                        ; break;

                    case EditorFieldAllType.GameObject:
                        builder.AppendLine($"        public {field.FieldType} {field.FieldName};");
                        ; break;

                    default:
                        builder.AppendLine($"        public {StringToTypeName(field.FieldType.ToString())} {field.FieldName};");
                        ; break;


                }
            }

            builder.AppendLine("    }");
            builder.AppendLine("}");

            string path = scriptableObjectEditor.CreateFilePath + $"/{ClassName}.cs";
            File.WriteAllText(path, builder.ToString());
            //AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            monoScript = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            builder.Clear();
        }


        public string StringToTypeName(string value) => value.ToLower() switch
        {
            "dictionary" => "dictionary",
            "list" => "List",
            "gameobject" => "GameObject",
            _ => value.ToLower()
        };

        [HorizontalGroup("A/B")]
        [LabelText("脚本"), LabelWidth(30)]
        [ReadOnly]
        public UnityEngine.Object monoScript;

        [HorizontalGroup("A/B")]
        [LabelText("展开"), LabelWidth(30)]
        public bool IsShow = false;


        [PropertySpace(5)]
        [LabelText("字段")]
        [ShowIf("IsShow")]
        [TableList]
        public List<EditorAssetField> fields = new List<EditorAssetField>();

        [InfoBox("命名冲突", InfoMessageType.Error, "@this.IsRepeatName()")]
        [VerticalGroup("资源")]
        [LabelText("新建资源"), LabelWidth(60)]
        [ShowIf("IsShow")]
        public string AssetName;

        [VerticalGroup("资源")]
        [Button("新建", ButtonSizes.Medium)]
        [ShowIf("@IsShow&&!System.String.IsNullOrEmpty(this.AssetName)&&!System.String.IsNullOrEmpty(scriptableObjectEditor.CreateAssetPath)")]
        public void CreateAsset()
        {
            if (monoScript==null)
            {
                if (IsCreateClass())
                {
                    CreateClassFile();
                }
            }else if (monoScript != null)
            {
                if (!IsRepeatName() && AssetName != "")
                {
                    ScriptableObject Asset = ScriptableObject.CreateInstance(ClassName);
                    string Folder = scriptableObjectEditor.CreateAssetPath + $"/{ClassName}/";
                    Directory.CreateDirectory(Folder);//如果文件夹不存在就创建它
                    AssetDatabase.CreateAsset(Asset, Folder + $"{AssetName}.asset");
                    Assets.Add(Asset);
                    AssetName = "";
                }
            }

        }


        [LabelText("资源列表")]
        [ShowIf("IsShow")]
        [InlineEditor]
        [Searchable]
        //[TableList(ShowIndexLabels = true)]
        [ListDrawerSettings(HideAddButton = true, CustomRemoveElementFunction = "RemoveAsset")]
        public List<ScriptableObject> Assets = new List<ScriptableObject>();

        [HideInInspector]
        public string ClassFilePath;

        public void RemoveAllAsset()
        {
            for (int i = 0; i < Assets.Count;)
            {
                if (Assets[i] != null)
                {
                    RemoveAsset(Assets[i]);
                    i++;
                }
            }
        }
        public void RemoveAsset(ScriptableObject scriptableObject)
        {
            Assets.Remove(scriptableObject);
            string path = AssetDatabase.GetAssetPath(scriptableObject);
            AssetDatabase.DeleteAsset(path);

            if (Assets.Count == 0)
            {
                AssetDatabase.DeleteAsset(Path.GetDirectoryName(path));
            }
        }



        public bool IsRepeatName()
        {
            return Assets.Any((item) => item.name == AssetName);
        }

        public bool IsRepeatFieldName()
        {
            return fields.Any((item) => string.IsNullOrEmpty(item.FieldName) || fields.Count((item1) => item1.FieldName == item.FieldName) != 1);
        }

        public bool IsCreateClass()
        {
           return !(string.IsNullOrEmpty(ClassName) || IsRepeatFieldName() ||string.IsNullOrEmpty(scriptableObjectEditor.CreateFilePath));
        }

    }

    [Serializable]
    public class EditorAssetField
    {
        [HideLabel, HorizontalGroup("类型")]
        public EditorFieldAllType FieldType;

        [ShowIf("FieldType", Value = EditorFieldAllType.Dictionary)]
        [HideLabel, HorizontalGroup("类型")]
        public EditorFieldType KeyType;

        [ShowIf("FieldType", Value = EditorFieldAllType.Dictionary)]
        [HideLabel, HorizontalGroup("类型")]
        public EditorFieldType ValueType;

        [ShowIf("FieldType", Value = EditorFieldAllType.List)]
        [HideLabel, HorizontalGroup("类型")]
        public EditorFieldType ItemType;

        [HideLabel, HorizontalGroup("类型")]
        [ShowIf("FieldType", Value = EditorFieldAllType.其它)]
        public string TypeName;

        [HideLabel, HorizontalGroup("名称")]
        public string FieldName;
        [HideLabel, HorizontalGroup("注释")]
        public string Comment;

    }


    public class EditorAssetBase : ScriptableObject
    {
    }
    [Serializable]
    public struct KeyValue<K, V>
    {
        [ShowInInspector]
        public K key;
        [ShowInInspector]
        public V value;
    }


    [Serializable]
    public class ListDictionary<Key, Value>
    {
        [TableList]
        public List<KeyValue<Key, Value>> List;
        [HideInInspector]
        private Dictionary<Key, Value> dictionary;
        [HideInInspector]
        public Dictionary<Key, Value> Dictionary
        {
            get
            {
                if (List.Count != Dictionary.Count)
                {
                    ToDictionary();
                }
                return dictionary;
            }
        }

        /// <summary>
        /// List转为字典
        /// </summary>
        public void ToDictionary()
        {
            foreach (var item in List)
            {
                dictionary.TryAdd(item.key, item.value);
            }
        }
    }



}
