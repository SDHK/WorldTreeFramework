using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace WorldTree
{
	public static partial class UnityNodeFieldViewRule
	{
		class BoundsViewRule : GenericsViewRule<Bounds>
		{ 
			protected override void OnEvent(UnityNodeFieldView<Bounds> self, INode node, FieldInfo arg1)
			{
				arg1.SetValue(node, EditorGUILayout.BoundsField(arg1.Name, (Bounds)arg1.GetValue(node)));
			}
		}
	}
}
