using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEditor;
using UnityEngine;
using WorldTree;
using System.Linq;

namespace EditorTool
{
	[CustomEditor(typeof(WorldTreeNodeMonoView))]
	public class WorldTreeNodeMonoViewEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			WorldTreeNodeMonoView treeNodeMonoView = (WorldTreeNodeMonoView)target;
			INode node = treeNodeMonoView.Node;
			INode View = treeNodeMonoView.View;
			try
			{
				EditorGUILayout.BeginVertical();

				EditorGUILayout.LongField("Id", node.Id);
				EditorGUILayout.LabelField("Type", node.ToString());

				FieldInfo[] fields = treeNodeMonoView.Node.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
				foreach (FieldInfo fieldInfo in fields)
				{
					Type type = fieldInfo.FieldType;

					if (View?.Root != null && View.Root.AddComponent(out ViewTypeManager _).types.TryGetValue(type, out Type nodeType))
					{
						View.Root.AddComponent(nodeType.TypeToCore(), out INode viewNode, isPool: false);
						viewNode.TrySendRule(TypeInfo<IWorldTreeNodeFieldInfoViewRule>.Default, node, fieldInfo);
					}
				}

				EditorGUILayout.EndVertical();
			}
			catch (Exception e)
			{
				Debug.Log($"component view error: {node.GetType().FullName} {e}");
			}
		}
	}
}
