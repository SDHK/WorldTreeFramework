using System.Reflection;
using UnityEditor;

namespace WorldTree
{
	public static partial class WorldTreeNodeFieldInfoUnityViewRule
	{
		class FloatViewRule : GenericsViewRule<float>
		{
			protected override void OnEvent(WorldTreeNodeFieldInfoUnityView<float> self, INode node, FieldInfo arg1)
			{
				arg1.SetValue(node, EditorGUILayout.FloatField(arg1.Name, (float)arg1.GetValue(node)));
			}
		}
	}
}
