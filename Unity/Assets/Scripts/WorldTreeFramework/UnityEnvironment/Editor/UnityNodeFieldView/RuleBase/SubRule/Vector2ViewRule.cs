using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace WorldTree
{
    public static partial class UnityNodeFieldViewRule
    {
        class Vector2ViewRule : GenericsViewRule<Vector2>
        {
            protected override void Execute(UnityNodeFieldView<Vector2> self, INode node, MemberInfo arg1)
            {
                if (arg1 is FieldInfo fieldInfo)
                {
                    var value = EditorGUILayout.Vector2Field(fieldInfo.Name, (Vector2)fieldInfo.GetValue(node));
                    fieldInfo.SetValue(node, value);
                }
                else if (arg1 is PropertyInfo propertyInfo)
                {
                    if (!propertyInfo.CanRead) return;
                    var value = EditorGUILayout.Vector2Field(propertyInfo.Name, (Vector2)propertyInfo.GetValue(node));
                    if (propertyInfo.CanWrite) propertyInfo.SetValue(node, value);
                }
            }
        }
    }
}
