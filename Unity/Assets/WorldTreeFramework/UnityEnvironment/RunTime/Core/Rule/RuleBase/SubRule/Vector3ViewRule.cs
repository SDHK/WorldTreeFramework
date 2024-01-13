using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace WorldTree
{
	public static partial class WorldTreeNodeFieldInfoUnityViewRule
	{
		class Vector3ViewRule : GenericsViewRule<Vector3>
		{
			protected override void OnEvent(WorldTreeNodeFieldInfoUnityView<Vector3> self, INode node, FieldInfo arg1)
			{
				arg1.SetValue(node, EditorGUILayout.Vector3Field(arg1.Name, (Vector3)arg1.GetValue(node)));
			}
		}
	}
}
