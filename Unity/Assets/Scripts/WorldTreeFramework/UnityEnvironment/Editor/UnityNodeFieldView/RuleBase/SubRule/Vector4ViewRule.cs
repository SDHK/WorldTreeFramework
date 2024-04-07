using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace WorldTree
{
	public static partial class UnityNodeFieldViewRule
	{
		class Vector4ViewRule : GenericsViewRule<Vector4>
		{
			protected override void Execute(UnityNodeFieldView<Vector4> self, INode node, FieldInfo arg1)
			{
				arg1.SetValue(node, EditorGUILayout.Vector4Field(arg1.Name, (Vector4)arg1.GetValue(node)));
			}
		}
	}
}
