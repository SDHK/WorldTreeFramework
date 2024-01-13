using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace WorldTree
{
	public static partial class WorldTreeNodeFieldInfoUnityViewRule
	{
		class Vector2ViewRule : GenericsViewRule<Vector2>
		{
			protected override void OnEvent(WorldTreeNodeFieldInfoUnityView<Vector2> self, INode node, FieldInfo arg1)
			{
				arg1.SetValue(node, EditorGUILayout.Vector2Field(arg1.Name, (Vector2)arg1.GetValue(node)));
			}
		}
	}
}
