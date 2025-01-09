using System;
using System.Reflection;
using UnityEditor;

namespace WorldTree
{
	public static partial class UnityNodeFieldViewRule
	{
		class ByteViewRule : GenericsViewRule<byte>
		{
			protected override void Execute(UnityNodeFieldView<byte> self, INode node, FieldInfo arg1)
			{
				// 将输入限制在 byte 范围
				int intValue = EditorGUILayout.IntField(arg1.Name, Convert.ToInt32(arg1.GetValue(node)));
				byte byteValue = (byte)Math.Clamp(intValue, byte.MinValue, byte.MaxValue);
				arg1.SetValue(node, byteValue);
			}
		}

		class SByteViewRule : GenericsViewRule<sbyte>
		{
			protected override void Execute(UnityNodeFieldView<sbyte> self, INode node, FieldInfo arg1)
			{
				// 将输入限制在 sbyte 范围
				int intValue = EditorGUILayout.IntField(arg1.Name, Convert.ToInt32(arg1.GetValue(node)));
				sbyte sbyteValue = (sbyte)Math.Clamp(intValue, sbyte.MinValue, sbyte.MaxValue);
				arg1.SetValue(node, sbyteValue);
			}
		}
	}
}
