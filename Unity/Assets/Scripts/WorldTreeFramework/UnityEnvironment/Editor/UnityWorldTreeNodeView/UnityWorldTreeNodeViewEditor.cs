/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：世界树节点可视化编辑器面板绘制

*/
using System;
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

				FieldInfo[] fields = monoView.Node.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
				foreach (FieldInfo fieldInfo in fields)
				{
					Type type = fieldInfo.FieldType;
					if (type.IsEnum) type = typeof(Enum);

					//通过字段类型拿到预注册的绘制节点类型
					if (View?.World != null && View.World.AddComponent(out ViewTypeManager _).types.TryGetValue(type, out Type nodeType))
					{
						//通过绘制类型拿到绘制节点实例
						long typeCode = View.TypeToCode(nodeType);
						NodeBranchHelper.AddNode<ComponentBranch, long>(View.World, typeCode, typeCode, out INode viewNode);

						//View.Root.AddComponent(nodeType.TypeToCode(), out INode viewNode, isPool: false);//通过绘制类型拿到绘制节点实例
						NodeRuleHelper.TrySendRule(viewNode, default(INodeFieldViewRule), node, fieldInfo);//调用绘制法则
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