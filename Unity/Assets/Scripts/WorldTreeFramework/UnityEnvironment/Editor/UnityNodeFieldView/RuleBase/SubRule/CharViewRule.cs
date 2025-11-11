using System.Reflection;
using UnityEditor;

namespace WorldTree
{
	public static partial class UnityNodeFieldViewRule
	{
		class CharViewRule : GenericsViewRule<char>
		{
			protected override void Execute(UnityNodeFieldView<char> self, INode node, MemberInfo arg1)
			{
				if (arg1 is FieldInfo fieldInfo)
				{
					var value = EditorGUILayout.TextField(fieldInfo.Name, fieldInfo.GetValue(node)?.ToString());
					if (!string.IsNullOrEmpty(value))
						fieldInfo.SetValue(node, value[0]);
				}
				else if (arg1 is PropertyInfo propertyInfo)
				{
					if (!propertyInfo.CanRead) return;
					var value = EditorGUILayout.TextField(propertyInfo.Name, propertyInfo.GetValue(node)?.ToString());
					if (!string.IsNullOrEmpty(value) && propertyInfo.CanWrite)
						propertyInfo.SetValue(node, value[0]);
				}
			}
		}
	}

}
