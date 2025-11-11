using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace WorldTree
{
    public static partial class UnityNodeFieldViewRule
    {
        class ColorViewRule : GenericsViewRule<Color>
        {
            protected override void Execute(UnityNodeFieldView<Color> self, INode node, MemberInfo arg1)
            {
                if (arg1 is FieldInfo fieldInfo)
                {
                    var value = EditorGUILayout.ColorField(fieldInfo.Name, (Color)fieldInfo.GetValue(node));
                    fieldInfo.SetValue(node, value);
                }
                else if (arg1 is PropertyInfo propertyInfo)
                {
                    if (!propertyInfo.CanRead) return;
                    var value = EditorGUILayout.ColorField(propertyInfo.Name, (Color)propertyInfo.GetValue(node));
                    if (propertyInfo.CanWrite) propertyInfo.SetValue(node, value);
                }
            }
        }
    }
}
