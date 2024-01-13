using System.Reflection;
using UnityEditor;

namespace WorldTree
{
	public static partial class WorldTreeNodeFieldInfoUnityViewRule
	{
		class BoolViewRule : GenericsViewRule<bool>
		{
			protected override void OnEvent(WorldTreeNodeFieldInfoUnityView<bool> self, INode node, FieldInfo arg1)
			{
				arg1.SetValue(node, EditorGUILayout.Toggle(arg1.Name, (bool)arg1.GetValue(node)));
			}
		}
	}
}
