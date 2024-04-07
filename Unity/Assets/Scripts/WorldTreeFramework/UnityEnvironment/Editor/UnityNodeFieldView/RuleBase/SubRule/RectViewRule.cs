
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace WorldTree
{
	public static partial class UnityNodeFieldViewRule
	{
		class RectViewRule : GenericsViewRule<Rect>
		{
			protected override void Execute(UnityNodeFieldView<Rect> self, INode node, FieldInfo arg1)
			{
				arg1.SetValue(node, EditorGUILayout.RectField(arg1.Name, (Rect)arg1.GetValue(node)));
			}
		}
	}
}
