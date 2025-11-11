using System.Reflection;
using UnityEditor;

namespace WorldTree
{
    public static partial class UnityNodeFieldViewRule
    {
        class StringViewRule : GenericsViewRule<string>
        {
            protected override void Execute(UnityNodeFieldView<string> self, INode node, MemberInfo arg1)
            {
                if (arg1 is FieldInfo fieldInfo)
                {
                    var value = EditorGUILayout.DelayedTextField(fieldInfo.Name, (string)fieldInfo.GetValue(node));
                    fieldInfo.SetValue(node, value);
                }
                else if (arg1 is PropertyInfo propertyInfo)
                {
                    if (!propertyInfo.CanRead) return;
                    var value = EditorGUILayout.DelayedTextField(propertyInfo.Name, (string)propertyInfo.GetValue(node));
                    if (propertyInfo.CanWrite) propertyInfo.SetValue(node, value);
                }
            }
        }
    }
}
