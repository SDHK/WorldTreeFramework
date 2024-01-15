using System.Reflection;
using UnityEditor;

namespace WorldTree
{
	public static partial class UnityNodeFieldViewRule
	{
		class BoolViewRule : GenericsViewRule<bool>
		{
			protected override void OnEvent(UnityNodeFieldView<bool> self, INode node, FieldInfo arg1)
			{
				arg1.SetValue(node, EditorGUILayout.Toggle(arg1.Name, (bool)arg1.GetValue(node)));
			}
		}
	}
}
