using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace WorldTree
{
	public static partial class WorldTreeNodeFieldInfoUnityViewRule
	{
		class ColorViewRule : GenericsViewRule<Color>
		{
			protected override void OnEvent(WorldTreeNodeFieldInfoUnityView<Color> self, INode node, FieldInfo arg1)
			{
				arg1.SetValue(node, EditorGUILayout.ColorField(arg1.Name, (Color)arg1.GetValue(node)));
			}
		}
	}
}
