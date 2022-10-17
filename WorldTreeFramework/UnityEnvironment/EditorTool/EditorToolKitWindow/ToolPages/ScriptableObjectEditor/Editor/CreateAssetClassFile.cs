
/****************************************

* 作者： 闪电黑客
* 日期： 2022/10/17 15:59

* 描述： EditorAssetClass的脚本文件 创建分类

*/

using System.IO;
using System.Text;
using UnityEditor;

namespace WorldTree
{
    /// <summary>
    /// 编辑字段的所有类型枚举
    /// </summary>
    public enum EditorFieldAllType
    {
        String,
        Byte,
        Short,
        Int,
        Long,
        Float,
        Double,
        Bool,
        Char,

        Dictionary,
        List,

        GameObject,
        Transform,

        Vector3,
        Vector2,
        Vector3Int,
        Vector2Int,
        Rect,
        RectInt,
        Color,
        AnimationCurve,

        其它

    }

    /// <summary>
    /// 编辑字段的类型枚举
    /// </summary>
    public enum EditorFieldType
    {
        String,
        Byte,
        Short,
        Int,
        Long,
        Float,
        Double,
        Bool,
        Char,

        GameObject,
        Transform,

        Vector3,
        Vector2,
        Vector3Int,
        Vector2Int,
        Rect,
        RectInt,
        Color,
        AnimationCurve,
    }

    public partial class EditorAssetClass
    {
        /// <summary>
        /// 枚举转类型名
        /// </summary>
        public string TypeNameToLower(string value)
        {
            switch (value)
            {
                case "String":
                case "Byte":
                case "Short":
                case "Int":
                case "Long":
                case "Float":
                case "Double":
                case "Bool":
                case "Char":
                    value = value.ToLower();
                    break;
            }
            return value;
        }

        /// <summary>
        /// 创建类文件
        /// </summary>
        public void CreateClassFile()
        {

            StringBuilder builder = new StringBuilder();

            builder.AppendLine("using System;");
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine("using UnityEngine;");

            builder.AppendLine($"namespace {scriptableObjectEditor.nameSpace}");
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
                        builder.AppendLine($"        public ListDictionary<{TypeNameToLower(field.KeyType.ToString())},{TypeNameToLower(field.ValueType.ToString())}> {field.FieldName};");
                        break;

                    case EditorFieldAllType.List:
                        builder.AppendLine($"        public List<{TypeNameToLower(field.ItemType.ToString())}> {field.FieldName};");
                        break;

                    case EditorFieldAllType.其它:
                        if (string.IsNullOrEmpty(field.TypeName)) field.TypeName = "string";
                        builder.AppendLine($"        public {field.TypeName} {field.FieldName};");
                        break;
                    default:
                        builder.AppendLine($"        public {TypeNameToLower(field.FieldType.ToString())} {field.FieldName};");
                        break;


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

        /// <summary>
        /// 创建列表类文件
        /// </summary>
        public void CreateListClassFile()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"namespace {scriptableObjectEditor.nameSpace}");
            builder.AppendLine("{");
            builder.AppendLine("    /// <summary>");
            builder.AppendLine($"    /// {ClassName}集合类");
            builder.AppendLine("    /// </summary>");
            builder.AppendLine($"   public partial class {ClassName}s:ListAssetBase<{ClassName}>" + "{}");
            builder.AppendLine("}");

            string path = scriptableObjectEditor.CreateFilePath + $"/{ClassName}s.cs";
            File.WriteAllText(path, builder.ToString());
            AssetDatabase.Refresh();
            monoListScript = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            builder.Clear();
        }


    }
}
