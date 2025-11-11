using System;
using System.Reflection;
using UnityEditor;

namespace WorldTree
{

	public static partial class UnityNodeFieldViewRule
	{
		class EnumViewRule : GenericsViewRule<Enum>
		{
			protected override void Execute(UnityNodeFieldView<Enum> self, INode node, MemberInfo arg1)
			{
				if (arg1 is FieldInfo fieldInfo)
				{
					var value = EditorGUILayout.EnumPopup(fieldInfo.Name, (Enum)fieldInfo.GetValue(node));
					fieldInfo.SetValue(node, value);
				}
				else if (arg1 is PropertyInfo propertyInfo)
				{
					if (!propertyInfo.CanRead) return;
					var value = EditorGUILayout.EnumPopup(propertyInfo.Name, (Enum)propertyInfo.GetValue(node));
					if (propertyInfo.CanWrite) propertyInfo.SetValue(node, value);
				}
			}
		}
	}
}
