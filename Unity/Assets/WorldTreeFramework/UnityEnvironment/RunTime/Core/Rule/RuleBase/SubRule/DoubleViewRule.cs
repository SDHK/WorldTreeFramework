using System.Reflection;
using UnityEditor;


namespace WorldTree
{
	public static partial class WorldTreeNodeFieldInfoUnityViewRule
	{
		class DoubleViewRule : GenericsViewRule<double>
		{
			protected override void OnEvent(WorldTreeNodeFieldInfoUnityView<double> self, INode node, FieldInfo arg1)
			{
				arg1.SetValue(node, EditorGUILayout.DoubleField(arg1.Name, (double)arg1.GetValue(node)));
			}
		}
	}
}
