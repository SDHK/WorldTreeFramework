using System;
using System.Reflection;
using UnityEditor;

namespace WorldTree
{
	public static partial class UnityNodeFieldViewRule
	{
		class ShortViewRule : GenericsViewRule<short>
		{
			protected override void Execute(UnityNodeFieldView<short> self, INode node, MemberInfo arg1)
			{
				if (arg1 is FieldInfo fieldInfo)
				{
					int intValue = EditorGUILayout.IntField(fieldInfo.Name, Convert.ToInt32(fieldInfo.GetValue(node)));
					short shortValue = (short)Math.Clamp(intValue, short.MinValue, short.MaxValue);
					fieldInfo.SetValue(node, shortValue);
				}
				else if (arg1 is PropertyInfo propertyInfo)
				{
					if (!propertyInfo.CanRead) return;
					int intValue = EditorGUILayout.IntField(propertyInfo.Name, Convert.ToInt32(propertyInfo.GetValue(node)));
					short shortValue = (short)Math.Clamp(intValue, short.MinValue, short.MaxValue);
					if (propertyInfo.CanWrite) propertyInfo.SetValue(node, shortValue);
				}
			}
		}

		class UShortViewRule : GenericsViewRule<ushort>
		{
			protected override void Execute(UnityNodeFieldView<ushort> self, INode node, MemberInfo arg1)
			{
				if (arg1 is FieldInfo fieldInfo)
				{
					int intValue = EditorGUILayout.IntField(fieldInfo.Name, Convert.ToInt32(fieldInfo.GetValue(node)));
					ushort ushortValue = (ushort)Math.Clamp(intValue, ushort.MinValue, ushort.MaxValue);
					fieldInfo.SetValue(node, ushortValue);
				}
				else if (arg1 is PropertyInfo propertyInfo)
				{
					if (!propertyInfo.CanRead) return;
					int intValue = EditorGUILayout.IntField(propertyInfo.Name, Convert.ToInt32(propertyInfo.GetValue(node)));
					ushort ushortValue = (ushort)Math.Clamp(intValue, ushort.MinValue, ushort.MaxValue);
					if (propertyInfo.CanWrite) propertyInfo.SetValue(node, ushortValue);
				}
			}
		}
	}
}
