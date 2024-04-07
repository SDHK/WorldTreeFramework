using System.Reflection;
using UnityEditor;

namespace WorldTree
{
	public static partial class UnityNodeFieldViewRule
	{
		class StringViewRule : GenericsViewRule<string>
		{
			protected override void Execute(UnityNodeFieldView<string> self, INode node, FieldInfo arg1)
			{
				arg1.SetValue(node, EditorGUILayout.DelayedTextField(arg1.Name, (string)arg1.GetValue(node)));
			}
		}
	}
}
