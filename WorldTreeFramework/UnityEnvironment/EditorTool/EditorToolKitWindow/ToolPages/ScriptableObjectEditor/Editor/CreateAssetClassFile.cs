
/****************************************

* 作者： 闪电黑客
* 日期： 2022/10/17 15:59

* 描述： EditorAssetClass的脚本文件 创建分类

*/

using System.IO;
using System;
using System.Text;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TextCore.Text;

namespace EditorTool
{
    /// <summary>
    /// 编辑字段的所有类型枚举
    /// </summary>
    public enum EditorFieldAllType
    {
        String,

        [LabelText("Byte    (0~255)")]
        Byte,
        [LabelText("Short   (-32768~327677)")]
        Short,
        [LabelText("Int     (+ -2147483647)")]
        Int,
        [LabelText("Long    64位,19位数")]
        Long,
        [LabelText("Float   32位,7位精度")]
        Float,
        [LabelText("Double  64位,15位精度")]
        Double,
        [LabelText("Decimal 128位,29位精度")]
        Decimal,

        [LabelText("Byte    (-128~127)")]
        sByte,
        [LabelText("uShot   (0~65536)")]
        uShot,
        [LabelText("uInt    (0~4294967295)")]
        uInt,
        [LabelText("uLong   无符号的20位数")]
        uLong,

        Bool,
        Char,

        Dictionary,
        List,
        Array,

        GameObject,
        Transform,

        Texture2D,
        AudioClip,
        Sprite,
        Material,
        Shader,
        ScriptableObject,
        TextAsset,

        Vector3,
        Vector2,
        Vector3Int,
        Vector2Int,
        Quaternion,
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

        [LabelText("Byte    (0~255)")]
        Byte,
        [LabelText("Short   (-32768~327677)")]
        Short,
        [LabelText("Int     (+ -2147483647)")]
        Int,
        [LabelText("Long    64位,19位数")]
        Long,
        [LabelText("Float   32位,7位精度")]
        Float,
        [LabelText("Double  64位,15位精度")]
        Double,
        [LabelText("Decimal 128位,29位精度")]
        Decimal,

        [LabelText("Byte    (-128~127)")]
        sByte,
        [LabelText("uShot   (0~65536)")]
        uShot,
        [LabelText("uInt    (0~4294967295)")]
        uInt,
        [LabelText("uLong   无符号的20位数")]
        uLong,

        Bool,
        Char,

        GameObject,
        Transform,

        Texture2D,
        AudioClip,
        Sprite,
        Material,
        Shader,
        ScriptableObject,
        TextAsset,

        Vector3,
        Vector2,
        Vector3Int,
        Vector2Int,
        Quaternion,
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
                case "Decimal":

                case "sByte":
                case "uShot":
                case "uInt":
                case "uLong":

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
                        builder.AppendLine($"        public List<{TypeNameToLower(field.ValueType.ToString())}> {field.FieldName};");
                        break;
                    case EditorFieldAllType.Array:
                        builder.AppendLine($"        public {TypeNameToLower(field.ValueType.ToString())}[] {field.FieldName};");
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
