using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace WorldTree
{
	public static partial class UnityNodeFieldViewRule
	{
		class Vector3ViewRule : GenericsViewRule<Vector3>
		{
			protected override void OnEvent(UnityNodeFieldView<Vector3> self, INode node, FieldInfo arg1)
			{
				arg1.SetValue(node, EditorGUILayout.Vector3Field(arg1.Name, (Vector3)arg1.GetValue(node)));
			}
		}
	}
}
