﻿using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace WorldTree
{
	public static partial class WorldTreeNodeFieldInfoUnityViewRule
	{
		class CharViewRule : GenericsViewRule<char>
		{
			protected override void OnEvent(WorldTreeNodeFieldInfoUnityView<char> self, INode node, FieldInfo arg1)
			{
				var str = EditorGUILayout.TextField(arg1.Name, ((char)arg1.GetValue(node)).ToString());
				arg1.SetValue(node, str.Length > 0 ? str[0] : default(char));
			}
		}
	}

}
