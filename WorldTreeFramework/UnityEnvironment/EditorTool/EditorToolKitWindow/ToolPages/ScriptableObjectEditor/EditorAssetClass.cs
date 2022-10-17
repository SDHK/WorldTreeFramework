using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

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

        [InfoBox("命名冲突", InfoMessageType.Error, "@this.IsRepeatAssetName()")]
        [VerticalGroup("资源")]
        [LabelText("新建资源"), LabelWidth(60)]
        [ShowIf("IsShow")]
        public string AssetName;

        [VerticalGroup("资源")]
        [Button("新建", ButtonSizes.Medium)]
        [ShowIf("@this.IsCreateAsset()")]
        public void CreateAsset()
        {
            if (monoScript == null)
            {
                if (IsCreateClass())
                {
                    CreateClassFile();
                }
            }
            else if (monoScript != null)
            {
                if (!IsRepeatAssetName() && AssetName != "")
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


       

        //判断字段名称为空或重复
        public bool IsRepeatFieldName()
        {
            return fields.Any((item) => string.IsNullOrEmpty(item.FieldName) || fields.Count((item1) => item1.FieldName == item.FieldName) != 1);
        }

        //判断是否可以新建类
        public bool IsCreateClass()
        {
            return !(string.IsNullOrEmpty(ClassName) || IsRepeatFieldName() || string.IsNullOrEmpty(scriptableObjectEditor.CreateFilePath));
        }

        //判断资源命名重复
        public bool IsRepeatAssetName()
        {
            return Assets.Any((item) => item.name == AssetName);
        }
        //判断是否可以新建资源
        public bool IsCreateAsset()
        {
            return IsShow && !(string.IsNullOrEmpty(AssetName) || string.IsNullOrEmpty(scriptableObjectEditor.CreateAssetPath) || IsRepeatAssetName());
        }

    }
}
