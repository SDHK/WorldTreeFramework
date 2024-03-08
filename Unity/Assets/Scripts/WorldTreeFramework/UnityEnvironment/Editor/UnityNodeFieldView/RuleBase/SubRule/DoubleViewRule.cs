using System.Reflection;
using UnityEditor;


namespace WorldTree
{
	public static partial class UnityNodeFieldViewRule
	{
		class DoubleViewRule : GenericsViewRule<double>
		{
			protected override void OnEvent(UnityNodeFieldView<double> self, INode node, FieldInfo arg1)
			{
				arg1.SetValue(node, EditorGUILayout.DoubleField(arg1.Name, (double)arg1.GetValue(node)));
			}
		}
	}
}
