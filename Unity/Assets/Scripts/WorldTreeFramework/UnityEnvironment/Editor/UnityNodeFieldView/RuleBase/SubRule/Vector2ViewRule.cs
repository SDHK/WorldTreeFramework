using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace WorldTree
{
	public static partial class UnityNodeFieldViewRule
	{
		class Vector2ViewRule : GenericsViewRule<Vector2>
		{
			protected override void Execute(UnityNodeFieldView<Vector2> self, INode node, FieldInfo arg1)
			{
				arg1.SetValue(node, EditorGUILayout.Vector2Field(arg1.Name, (Vector2)arg1.GetValue(node)));
			}
		}
	}
}
