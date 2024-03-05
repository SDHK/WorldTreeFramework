﻿using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using WorldTree;

namespace EditorTool
{
	[CustomEditor(typeof(WorldTreeNodeViewMonoComponent))]
	public class WorldTreeNodeViewMonoComponentEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			WorldTreeNodeViewMonoComponent monoView = (WorldTreeNodeViewMonoComponent)target;
			INode node = monoView.Node;
			INode View = monoView.View;
			try
			{
				EditorGUILayout.BeginVertical();

				EditorGUILayout.LabelField("Type", node.ToString());
				EditorGUILayout.LongField("Id", node.Id);
				EditorGUILayout.Toggle("IsActive", node.IsActive);
				EditorGUILayout.Space();
				node.SetActive(EditorGUILayout.Toggle("ActiveToggle", node.ActiveToggle));
				if (node.IsActive != monoView.gameObject.activeSelf)
				{
					monoView.gameObject.SetActive(node.IsActive);
				}

				FieldInfo[] fields = monoView.Node.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
				foreach (FieldInfo fieldInfo in fields)
				{
					Type type = fieldInfo.FieldType;

					//通过字段类型拿到预注册的绘制节点类型
					if (View?.Root != null && View.Root.AddComponent(out ViewTypeManager _).types.TryGetValue(type, out Type nodeType))
					{
						View.Root.AddComponent(nodeType.TypeToCore(), out INode viewNode, isPool: false);//通过绘制类型拿到绘制节点实例
						viewNode.TrySendRule(TypeInfo<INodeFieldViewRule>.Default, node, fieldInfo);//调用绘制法则
					}
				}

				EditorGUILayout.EndVertical();
			}
			catch (Exception e)
			{
				Debug.Log($"Node View error: {node.GetType().FullName} {e}");
			}
		}
	}
}