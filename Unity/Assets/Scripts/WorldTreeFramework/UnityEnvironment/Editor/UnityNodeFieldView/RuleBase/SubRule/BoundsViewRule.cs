using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace WorldTree
{
    public static partial class UnityNodeFieldViewRule
    {
        class BoundsViewRule : GenericsViewRule<Bounds>
        {
            protected override void Execute(UnityNodeFieldView<Bounds> self, INode node, MemberInfo arg1)
            {
                if (arg1 is FieldInfo fieldInfo)
                {
                    var value = EditorGUILayout.BoundsField(fieldInfo.Name, (Bounds)fieldInfo.GetValue(node));
                    fieldInfo.SetValue(node, value);
                }
                else if (arg1 is PropertyInfo propertyInfo)
                {
                    if (!propertyInfo.CanRead) return;
                    var value = EditorGUILayout.BoundsField(propertyInfo.Name, (Bounds)propertyInfo.GetValue(node));
                    if (propertyInfo.CanWrite) propertyInfo.SetValue(node, value);
                }
            }
        }
    }
}
