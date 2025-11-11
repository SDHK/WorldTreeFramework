using System.Reflection;
using UnityEditor;

namespace WorldTree
{
	public static partial class UnityNodeFieldViewRule
	{
		class BoolViewRule : GenericsViewRule<bool>
		{
			protected override void Execute(UnityNodeFieldView<bool> self, INode node, MemberInfo arg1)
			{
				if (arg1 is FieldInfo fieldInfo)
				{
					var value = EditorGUILayout.Toggle(fieldInfo.Name, (bool)fieldInfo.GetValue(node));
					fieldInfo.SetValue(node, value);
				}
				else if (arg1 is PropertyInfo propertyInfo)
				{
					if (!propertyInfo.CanRead) return;
					var value = EditorGUILayout.Toggle(propertyInfo.Name, (bool)propertyInfo.GetValue(node));
					if (propertyInfo.CanWrite) propertyInfo.SetValue(node, value);
				}
			}
		}
	}
}
