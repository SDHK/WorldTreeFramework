using System.Reflection;
using UnityEditor;

namespace WorldTree
{
    public static partial class UnityNodeFieldViewRule
    {
        class FloatViewRule : GenericsViewRule<float>
        {
            protected override void Execute(UnityNodeFieldView<float> self, INode node, MemberInfo arg1)
            {
                if (arg1 is FieldInfo fieldInfo)
                {
                    var value = EditorGUILayout.FloatField(fieldInfo.Name, (float)fieldInfo.GetValue(node));
                    fieldInfo.SetValue(node, value);
                }
                else if (arg1 is PropertyInfo propertyInfo)
                {
                    if (!propertyInfo.CanRead) return;
                    var value = EditorGUILayout.FloatField(propertyInfo.Name, (float)propertyInfo.GetValue(node));
                    if (propertyInfo.CanWrite) propertyInfo.SetValue(node, value);
                }
            }
        }
    }
}
