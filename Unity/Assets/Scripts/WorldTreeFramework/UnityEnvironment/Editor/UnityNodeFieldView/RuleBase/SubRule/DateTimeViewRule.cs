using System;
using System.Reflection;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor;
using UnityEngine;

namespace WorldTree
{
	public static partial class UnityNodeFieldViewRule

	{
		class DateTimeViewRule : GenericsViewRule<DateTime>
		{
			protected override void OnEvent(UnityNodeFieldView<DateTime> self, INode node, FieldInfo arg1)
			{
				var dateString = arg1.GetValue(node).ToString();
				var newDateString = EditorGUILayout.TextField(arg1.Name, dateString);
				if (dateString != newDateString) arg1.SetValue(node, DateTime.Parse(newDateString));
			}
		}
	}
}
