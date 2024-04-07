using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace WorldTree
{
	public static partial class UnityNodeFieldViewRule
	{
		class ColorViewRule : GenericsViewRule<Color>
		{
			protected override void Execute(UnityNodeFieldView<Color> self, INode node, FieldInfo arg1)
			{
				arg1.SetValue(node, EditorGUILayout.ColorField(arg1.Name, (Color)arg1.GetValue(node)));
			}
		}
	}
}
