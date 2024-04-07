using System.Reflection;
using UnityEditor;

namespace WorldTree
{
	public static partial class UnityNodeFieldViewRule
	{
		class FloatViewRule : GenericsViewRule<float>
		{
			protected override void Execute(UnityNodeFieldView<float> self, INode node, FieldInfo arg1)
			{
				arg1.SetValue(node, EditorGUILayout.FloatField(arg1.Name, (float)arg1.GetValue(node)));
			}
		}
	}
}
