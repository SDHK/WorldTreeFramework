using System;
using System.Reflection;
using UnityEditor;

namespace WorldTree
{

	public static partial class WorldTreeNodeFieldInfoUnityViewRule
	{
		class EnumViewRule : GenericsViewRule<Enum>
		{
			protected override void OnEvent(WorldTreeNodeFieldInfoUnityView<Enum> self, INode node, FieldInfo arg1)
			{
				if (arg1.FieldType.IsDefined(typeof(FlagsAttribute), false))
				{
					arg1.SetValue(node, EditorGUILayout.EnumFlagsField(arg1.Name, (Enum)arg1.GetValue(node)));
				}
				else
				{
					arg1.SetValue(node, EditorGUILayout.EnumPopup(arg1.Name, (Enum)arg1.GetValue(node)));
				}
			}
		}
	}
}
