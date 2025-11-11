using System;
using System.Reflection;
using UnityEditor;

namespace WorldTree
{
	public static partial class UnityNodeFieldViewRule
	{
		class IntViewRule : GenericsViewRule<int>
		{
			protected override void Execute(UnityNodeFieldView<int> self, INode node, MemberInfo arg1)
			{
				if (arg1 is FieldInfo fieldInfo)
				{
					var value = EditorGUILayout.IntField(fieldInfo.Name, Convert.ToInt32(fieldInfo.GetValue(node)));
					fieldInfo.SetValue(node, value);
				}
				else if (arg1 is PropertyInfo propertyInfo)
				{
					if (!propertyInfo.CanRead) return;
					var value = EditorGUILayout.IntField(propertyInfo.Name, Convert.ToInt32(propertyInfo.GetValue(node)));
					if (propertyInfo.CanWrite) propertyInfo.SetValue(node, value);
				}
			}
		}


		class UIntViewRule : GenericsViewRule<uint>
		{
			protected override void Execute(UnityNodeFieldView<uint> self, INode node, MemberInfo arg1)
			{
				if (arg1 is FieldInfo fieldInfo)
				{
					long longValue = EditorGUILayout.LongField(fieldInfo.Name, Convert.ToInt64(fieldInfo.GetValue(node)));
					uint uintValue = (uint)Math.Clamp(longValue, uint.MinValue, uint.MaxValue);
					fieldInfo.SetValue(node, uintValue);
				}
				else if (arg1 is PropertyInfo propertyInfo)
				{
					if (!propertyInfo.CanRead) return;
					long longValue = EditorGUILayout.LongField(propertyInfo.Name, Convert.ToInt64(propertyInfo.GetValue(node)));
					uint uintValue = (uint)Math.Clamp(longValue, uint.MinValue, uint.MaxValue);
					if (propertyInfo.CanWrite) propertyInfo.SetValue(node, uintValue);
				}
			}
		}


	}
}
