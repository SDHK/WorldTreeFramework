using System.Reflection;
using UnityEditor;

namespace WorldTree
{
	public static partial class WorldTreeNodeFieldInfoUnityViewRule
	{
		class IntViewRule : GenericsViewRule<int>
		{
			protected override void OnEvent(WorldTreeNodeFieldInfoUnityView<int> self, INode node, FieldInfo arg1)
			{
				arg1.SetValue(node, EditorGUILayout.IntField(arg1.Name, (int)arg1.GetValue(node)));
			}
		}
	}
}
