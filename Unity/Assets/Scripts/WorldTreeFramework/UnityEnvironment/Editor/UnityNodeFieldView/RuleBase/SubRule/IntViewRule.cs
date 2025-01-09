using System;
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
				arg1.SetValue(node, EditorGUILayout.IntField(arg1.Name, Convert.ToInt32(arg1.GetValue(node))));
			}
		}


		class UIntViewRule : GenericsViewRule<uint>
		{
			protected override void Execute(UnityNodeFieldView<uint> self, INode node, FieldInfo arg1)
			{
				// 将输入限制在 uint 范围
				long longValue = EditorGUILayout.LongField(arg1.Name, Convert.ToInt64(arg1.GetValue(node)));
				uint uintValue = (uint)Math.Clamp(longValue, uint.MinValue, uint.MaxValue);
				arg1.SetValue(node, uintValue);
			}
		}


	}
}
