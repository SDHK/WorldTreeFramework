using System;
using System.Reflection;
using UnityEditor;

namespace WorldTree
{
	public static partial class UnityNodeFieldViewRule
	{
		class ShortViewRule : GenericsViewRule<short>
		{
			protected override void Execute(UnityNodeFieldView<short> self, INode node, FieldInfo arg1)
			{
				// 将输入限制在 short 范围
				int intValue = EditorGUILayout.IntField(arg1.Name, Convert.ToInt32(arg1.GetValue(node)));
				short shortValue = (short)Math.Clamp(intValue, short.MinValue, short.MaxValue);
				arg1.SetValue(node, shortValue);
			}
		}

		class UShortViewRule : GenericsViewRule<ushort>
		{
			protected override void Execute(UnityNodeFieldView<ushort> self, INode node, FieldInfo arg1)
			{
				// 将输入限制在 ushort 范围
				int intValue = EditorGUILayout.IntField(arg1.Name, Convert.ToInt32(arg1.GetValue(node)));
				ushort ushortValue = (ushort)Math.Clamp(intValue, ushort.MinValue, ushort.MaxValue);
				arg1.SetValue(node, ushortValue);
			}
		}
	}
}
