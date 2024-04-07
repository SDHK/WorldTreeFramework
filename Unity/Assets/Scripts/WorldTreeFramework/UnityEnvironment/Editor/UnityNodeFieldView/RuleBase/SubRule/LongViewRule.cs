using System.Reflection;
using UnityEditor;

namespace WorldTree
{
	public static partial class UnityNodeFieldViewRule
	{
		class LongViewRule : GenericsViewRule<long>
		{
			protected override void Execute(UnityNodeFieldView<long> self, INode node, FieldInfo arg1)
			{
				arg1.SetValue(node, EditorGUILayout.LongField(arg1.Name, (long)arg1.GetValue(node)));
			}
		}
	}
}
