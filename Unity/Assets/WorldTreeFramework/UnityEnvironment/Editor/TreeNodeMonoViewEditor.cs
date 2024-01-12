using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEditor;
using UnityEngine;
using WorldTree;

namespace EditorTool
{
	[CustomEditor(typeof(TreeNodeMonoView))]
	public class TreeNodeMonoViewEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			TreeNodeMonoView treeNodeMonoView = (TreeNodeMonoView)target;
			INode node = treeNodeMonoView.Node;
			EditorGUILayout.BeginVertical();

			EditorGUILayout.LongField("Id", node.Id);
			EditorGUILayout.LabelField("Type", node.ToString());

			EditorGUILayout.EndVertical();

		}
	}

	public static class TreeNodeViewHelper
	{
		private static readonly WorldTreeCore Core;

		static TreeNodeViewHelper()
		{
			Core = new WorldTreeCore();
			Core.Log = Debug.Log;
			Core.LogWarning = Debug.LogWarning;
			Core.LogError = Debug.LogError;
			Core.Awake();
		}
	}
}
