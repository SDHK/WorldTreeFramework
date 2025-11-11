using System;
using System.Reflection;
using UnityEditor;

namespace WorldTree
{
    public static partial class UnityNodeFieldViewRule
    {
        class ByteViewRule : GenericsViewRule<byte>
        {
            protected override void Execute(UnityNodeFieldView<byte> self, INode node, MemberInfo arg1)
            {
                if (arg1 is FieldInfo fieldInfo)
                {
                    int intValue = EditorGUILayout.IntField(fieldInfo.Name, Convert.ToInt32(fieldInfo.GetValue(node)));
                    byte byteValue = (byte)Math.Clamp(intValue, byte.MinValue, byte.MaxValue);
                    fieldInfo.SetValue(node, byteValue);
                }
                else if (arg1 is PropertyInfo propertyInfo)
                {
                    if (!propertyInfo.CanRead) return;
                    int intValue = EditorGUILayout.IntField(propertyInfo.Name, Convert.ToInt32(propertyInfo.GetValue(node)));
                    byte byteValue = (byte)Math.Clamp(intValue, byte.MinValue, byte.MaxValue);
                    if (propertyInfo.CanWrite) propertyInfo.SetValue(node, byteValue);
                }
            }
        }

        class SByteViewRule : GenericsViewRule<sbyte>
        {
            protected override void Execute(UnityNodeFieldView<sbyte> self, INode node, MemberInfo arg1)
            {
                if (arg1 is FieldInfo fieldInfo)
                {
                    int intValue = EditorGUILayout.IntField(fieldInfo.Name, Convert.ToInt32(fieldInfo.GetValue(node)));
                    sbyte sbyteValue = (sbyte)Math.Clamp(intValue, sbyte.MinValue, sbyte.MaxValue);
                    fieldInfo.SetValue(node, sbyteValue);
                }
                else if (arg1 is PropertyInfo propertyInfo)
                {
                    if (!propertyInfo.CanRead) return;
                    int intValue = EditorGUILayout.IntField(propertyInfo.Name, Convert.ToInt32(propertyInfo.GetValue(node)));
                    sbyte sbyteValue = (sbyte)Math.Clamp(intValue, sbyte.MinValue, sbyte.MaxValue);
                    if (propertyInfo.CanWrite) propertyInfo.SetValue(node, sbyteValue);
                }
            }
        }
    }
}
