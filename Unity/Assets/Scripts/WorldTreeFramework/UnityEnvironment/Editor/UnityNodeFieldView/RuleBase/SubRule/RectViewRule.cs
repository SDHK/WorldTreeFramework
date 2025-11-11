using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace WorldTree
{
	public static partial class UnityNodeFieldViewRule
	{
		class RectViewRule : GenericsViewRule<Rect>
		{
			protected override void Execute(UnityNodeFieldView<Rect> self, INode node, MemberInfo arg1)
			{
				if (arg1 is FieldInfo fieldInfo)
				{
					var value = EditorGUILayout.RectField(fieldInfo.Name, (Rect)fieldInfo.GetValue(node));
					fieldInfo.SetValue(node, value);
				}
				else if (arg1 is PropertyInfo propertyInfo)
				{
					if (!propertyInfo.CanRead) return;
					var value = EditorGUILayout.RectField(propertyInfo.Name, (Rect)propertyInfo.GetValue(node));
					if (propertyInfo.CanWrite) propertyInfo.SetValue(node, value);
				}
			}
		}
	}
}
