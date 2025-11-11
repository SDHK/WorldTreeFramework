using System.Reflection;
using UnityEditor;

namespace WorldTree
{
	public static partial class UnityNodeFieldViewRule
	{
		class LongViewRule : GenericsViewRule<long>
		{
			protected override void Execute(UnityNodeFieldView<long> self, INode node, MemberInfo arg1)
			{
				if (arg1 is FieldInfo fieldInfo)
				{
					var value = EditorGUILayout.LongField(fieldInfo.Name, (long)fieldInfo.GetValue(node));
					fieldInfo.SetValue(node, value);
				}
				else if (arg1 is PropertyInfo propertyInfo)
				{
					if (!propertyInfo.CanRead) return;
					var value = EditorGUILayout.LongField(propertyInfo.Name, (long)propertyInfo.GetValue(node));
					if (propertyInfo.CanWrite) propertyInfo.SetValue(node, value);
				}
			}
		}

		class ULongViewRule : GenericsViewRule<ulong>
		{
			protected override void Execute(UnityNodeFieldView<ulong> self, INode node, MemberInfo arg1)
			{
				if (arg1 is FieldInfo fieldInfo)
				{
					long longValue = EditorGUILayout.LongField(fieldInfo.Name, (long)fieldInfo.GetValue(node));
					ulong ulongValue = (ulong)System.Math.Clamp(longValue, (long)ulong.MinValue, long.MaxValue);
					fieldInfo.SetValue(node, ulongValue);
				}
				else if (arg1 is PropertyInfo propertyInfo)
				{
					if (!propertyInfo.CanRead) return;
					long longValue = EditorGUILayout.LongField(propertyInfo.Name, (long)propertyInfo.GetValue(node));
					ulong ulongValue = (ulong)System.Math.Clamp(longValue, (long)ulong.MinValue, long.MaxValue);
					if (propertyInfo.CanWrite) propertyInfo.SetValue(node, ulongValue);
				}
			}
		}
	}
}
