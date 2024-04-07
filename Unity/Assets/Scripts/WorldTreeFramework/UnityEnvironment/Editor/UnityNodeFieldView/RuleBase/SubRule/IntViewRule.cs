using System.Reflection;
using UnityEditor;

namespace WorldTree
{
	public static partial class UnityNodeFieldViewRule
	{
		class IntViewRule : GenericsViewRule<int>
		{
			protected override void Execute(UnityNodeFieldView<int> self, INode node, FieldInfo arg1)
			{
				arg1.SetValue(node, EditorGUILayout.IntField(arg1.Name, (int)arg1.GetValue(node)));
			}
		}
	}
}
