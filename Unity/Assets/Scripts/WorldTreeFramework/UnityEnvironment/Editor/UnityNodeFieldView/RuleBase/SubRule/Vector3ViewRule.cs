using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace WorldTree
{
    public static partial class UnityNodeFieldViewRule
    {
        class Vector3ViewRule : GenericsViewRule<Vector3>
        {
            protected override void Execute(UnityNodeFieldView<Vector3> self, INode node, MemberInfo arg1)
            {
                if (arg1 is FieldInfo fieldInfo)
                {
                    var value = EditorGUILayout.Vector3Field(fieldInfo.Name, (Vector3)fieldInfo.GetValue(node));
                    fieldInfo.SetValue(node, value);
                }
                else if (arg1 is PropertyInfo propertyInfo)
                {
                    if (!propertyInfo.CanRead) return;
                    var value = EditorGUILayout.Vector3Field(propertyInfo.Name, (Vector3)propertyInfo.GetValue(node));
                    if (propertyInfo.CanWrite) propertyInfo.SetValue(node, value);
                }
            }
        }
    }
}
