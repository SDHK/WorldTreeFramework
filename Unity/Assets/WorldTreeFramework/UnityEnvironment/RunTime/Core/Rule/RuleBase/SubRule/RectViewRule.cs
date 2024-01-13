
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace WorldTree
{
	public static partial class WorldTreeNodeFieldInfoUnityViewRule
	{
		class RectViewRule : GenericsViewRule<Rect>
		{
			protected override void OnEvent(WorldTreeNodeFieldInfoUnityView<Rect> self, INode node, FieldInfo arg1)
			{
				arg1.SetValue(node, EditorGUILayout.RectField(arg1.Name, (Rect)arg1.GetValue(node)));
			}
		}
	}
}
