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

		class ULongViewRule : GenericsViewRule<ulong>
		{
			protected override void Execute(UnityNodeFieldView<ulong> self, INode node, FieldInfo arg1)
			{
				// 将输入限制在 ulong 范围
				ulong ulongValue = (ulong)EditorGUILayout.LongField(arg1.Name, (long)arg1.GetValue(node));
				arg1.SetValue(node, ulongValue);
			}
		}
	}
}
