using System.Reflection;
using UnityEditor;

namespace WorldTree
{
	public static partial class WorldTreeNodeFieldInfoUnityViewRule
	{
		class StringViewRule : GenericsViewRule<string>
		{
			protected override void OnEvent(WorldTreeNodeFieldInfoUnityView<string> self, INode node, FieldInfo arg1)
			{
				arg1.SetValue(node, EditorGUILayout.DelayedTextField(arg1.Name, (string)arg1.GetValue(node)));
			}
		}
	}
}
