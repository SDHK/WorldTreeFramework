
/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/28 9:54

* 描述： 继承Mono的编辑器窗体
* 
* 可塞入实体获得生命周期进行绘制
* 

*/

using UnityEditor;
using WorldTree;

namespace EditorTool
{
	/// <summary>
	/// 继承Mono的编辑器窗体
	/// </summary>
	public class MonoEditorGUIWindow : EditorWindow
	{
		public INode entity;

		public bool isShow = false;


		private void OnEnable()
		{
			if (!isShow)
			{
				isShow = true;
			}
			NodeRuleHelper.TrySendRule(entity, TypeInfo<IEditorWindowEnableSystem>.Default);
		}

		private void OnFocus()
		{
			NodeRuleHelper.TrySendRule(entity, TypeInfo<IEditorWindowFocusSystem>.Default);
		}

		private void OnInspectorUpdate()
		{
			NodeRuleHelper.TrySendRule(entity, TypeInfo<IEditorWindowInspectorUpdateSystem>.Default);
		}

		public void OnProjectChange()
		{
			NodeRuleHelper.TrySendRule(entity, TypeInfo<IEditorWindowProjectChangeSystem>.Default);
		}

		private void OnSelectionChange()
		{
			NodeRuleHelper.TrySendRule(entity, TypeInfo<IEditorWindowSelectionChangeSystem>.Default);
		}

		public void OnHierarchyChange()
		{
			NodeRuleHelper.TrySendRule(entity, TypeInfo<IEditorWindowHierarchyChangeSystem>.Default);
		}
		private void OnGUI()
		{
			NodeRuleHelper.TrySendRule(entity, TypeInfo<GUIDraw>.Default);
		}

		private void OnLostFocus()
		{
			NodeRuleHelper.TrySendRule(entity, TypeInfo<IEditorWindowLostFocusSystem>.Default);
		}
		private void OnDisable()
		{
			NodeRuleHelper.TrySendRule(entity, TypeInfo<IEditorWindowDisableSystem>.Default);
		}
		private void OnDestroy()
		{
			NodeRuleHelper.TrySendRule(entity, TypeInfo<IEditorWindowDestroySystem>.Default);
			if (isShow)
			{
				isShow = false;
				entity?.Dispose();
			}
		}
	}
}

