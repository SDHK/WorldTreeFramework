using System.Reflection;
using UnityEditor;


namespace WorldTree
{
	public static partial class UnityNodeFieldViewRule
	{
		class DoubleViewRule : GenericsViewRule<double>
		{
			protected override void Execute(UnityNodeFieldView<double> self, INode node, MemberInfo arg1)
			{
				if (arg1 is FieldInfo fieldInfo)
				{
					var value = EditorGUILayout.DoubleField(fieldInfo.Name, (double)fieldInfo.GetValue(node));
					fieldInfo.SetValue(node, value);
				}
				else if (arg1 is PropertyInfo propertyInfo)
				{
					if (!propertyInfo.CanRead) return;
					var value = EditorGUILayout.DoubleField(propertyInfo.Name, (double)propertyInfo.GetValue(node));
					if (propertyInfo.CanWrite) propertyInfo.SetValue(node, value);
				}
			}
		}
	}
}
