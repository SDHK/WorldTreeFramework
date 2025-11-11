using System;
using System.Reflection;
using UnityEditor;

namespace WorldTree
{
    public static partial class UnityNodeFieldViewRule
    {
        class DateTimeViewRule : GenericsViewRule<DateTime>
        {
            protected override void Execute(UnityNodeFieldView<DateTime> self, INode node, MemberInfo arg1)
            {
                if (arg1 is FieldInfo fieldInfo)
                {
                    var dateString = fieldInfo.GetValue(node).ToString();
                    var newDateString = EditorGUILayout.TextField(fieldInfo.Name, dateString);
                    if (dateString != newDateString)
                    {
                        if (DateTime.TryParse(newDateString, out var newDate))
                            fieldInfo.SetValue(node, newDate);
                    }
                }
                else if (arg1 is PropertyInfo propertyInfo)
                {
                    if (!propertyInfo.CanRead) return;
                    var dateString = propertyInfo.GetValue(node)?.ToString();
                    var newDateString = EditorGUILayout.TextField(propertyInfo.Name, dateString);
                    if (dateString != newDateString && propertyInfo.CanWrite)
                    {
                        if (DateTime.TryParse(newDateString, out var newDate))
                            propertyInfo.SetValue(node, newDate);
                    }
                }
            }
        }
    }
}
