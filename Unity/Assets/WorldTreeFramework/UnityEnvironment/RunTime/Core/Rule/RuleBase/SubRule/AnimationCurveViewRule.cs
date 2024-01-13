﻿using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace WorldTree
{
	public static partial class WorldTreeNodeFieldInfoUnityViewRule
	{
		class AnimationCurveViewRule : GenericsViewRule<AnimationCurve>
		{
			protected override void OnEvent(WorldTreeNodeFieldInfoUnityView<AnimationCurve> self, INode arg1, FieldInfo arg2)
			{
				arg2.SetValue(arg1, EditorGUILayout.CurveField(arg2.Name, (AnimationCurve)arg2.GetValue(arg1)));
			}
		}
	}
}
