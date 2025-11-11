using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace WorldTree
{
    public static partial class UnityNodeFieldViewRule
    {
        class Vector4ViewRule : GenericsViewRule<Vector4>
        {
            protected override void Execute(UnityNodeFieldView<Vector4> self, INode node, MemberInfo arg1)
            {
                if (arg1 is FieldInfo fieldInfo)
                {
                    var value = EditorGUILayout.Vector4Field(fieldInfo.Name, (Vector4)fieldInfo.GetValue(node));
                    fieldInfo.SetValue(node, value);
                }
                else if (arg1 is PropertyInfo propertyInfo)
                {
                    if (!propertyInfo.CanRead) return;
                    var value = EditorGUILayout.Vector4Field(propertyInfo.Name, (Vector4)propertyInfo.GetValue(node));
                    if (propertyInfo.CanWrite) propertyInfo.SetValue(node, value);
                }
            }
        }
    }
}
