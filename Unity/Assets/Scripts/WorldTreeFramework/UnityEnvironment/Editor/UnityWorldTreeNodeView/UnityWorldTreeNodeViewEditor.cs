/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：世界树节点可视化编辑器面板绘制

*/
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using WorldTree;

namespace EditorTool
{
	/// <summary>
	/// 世界树节点可视化编辑器面板绘制
	/// </summary>
	[CustomEditor(typeof(UnityWorldTreeNodeView))]
	public class UnityWorldTreeNodeViewEditor : Editor
	{
		static List<MemberInfo> members = new();

		public override void OnInspectorGUI()
		{
			UnityWorldTreeNodeView monoView = (UnityWorldTreeNodeView)target;
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

				members.Clear();
				var type = monoView.Node.GetType();
				var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
				var properties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

				foreach (var field in fields)
				{
					if (field.Name.Contains("k__BackingField")) continue;
					members.Add(field);
				}
				foreach (var property in properties)
				{
					members.Add(property);
				}

				// 按声明顺序排序
				//members.Sort((a, b) => a.MetadataToken.CompareTo(b.MetadataToken));

				foreach (MemberInfo memberInfo in members)
				{
					type = null;

					if (memberInfo is FieldInfo fieldInfo)
					{
						type = fieldInfo.FieldType;
					}
					else if (memberInfo is PropertyInfo propertyInfo)
					{
						type = propertyInfo.PropertyType;
					}
					else
					{
						continue;
					}


					if (type.IsEnum) type = typeof(Enum);

					if (View?.World == null) continue;
					if (!View.World.TryGetComponent(out ViewTypeManager viewTypeManager))
					{
						View.World.AddComponent(out viewTypeManager);
					}

					//通过字段类型拿到预注册的绘制节点类型
					if (!viewTypeManager.types.TryGetValue(type, out Type nodeType)) continue;

					//通过绘制类型拿到绘制节点实例
					long typeCode = View.TypeToCode(nodeType);

					if (!View.World.ComponentBranch().TryGetNode(typeCode, out INode viewNode))
					{
						NodeBranchHelper.AddNode(View.World, default(ComponentBranch), typeCode, typeCode, out viewNode);
					}
					NodeRuleHelper.TrySendRule(viewNode, default(INodeFieldViewRule), node, memberInfo);//调用绘制法则
				}

				EditorGUILayout.EndVertical();
			}
			catch (Exception e)
			{
				Debug.LogError($"Node View error: {node.GetType().FullName} {e}");
			}
		}
	}
}