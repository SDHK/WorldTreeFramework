
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
			NodeRuleHelper.TrySendRule(entity, default(IEditorWindowEnableSystem));
		}

		private void OnFocus()
		{
			NodeRuleHelper.TrySendRule(entity, default(IEditorWindowFocusSystem));
		}

		private void OnInspectorUpdate()
		{
			NodeRuleHelper.TrySendRule(entity, default(IEditorWindowInspectorUpdateSystem));
		}

		public void OnProjectChange()
		{
			NodeRuleHelper.TrySendRule(entity, default(IEditorWindowProjectChangeSystem));
		}

		private void OnSelectionChange()
		{
			NodeRuleHelper.TrySendRule(entity, default(IEditorWindowSelectionChangeSystem));
		}

		public void OnHierarchyChange()
		{
			NodeRuleHelper.TrySendRule(entity, default(IEditorWindowHierarchyChangeSystem));
		}
		private void OnGUI()
		{
			NodeRuleHelper.TrySendRule(entity, default(GUIDraw));
		}

		private void OnLostFocus()
		{
			NodeRuleHelper.TrySendRule(entity, default(IEditorWindowLostFocusSystem));
		}
		private void OnDisable()
		{
			NodeRuleHelper.TrySendRule(entity, default(IEditorWindowDisableSystem));
		}
		private void OnDestroy()
		{
			NodeRuleHelper.TrySendRule(entity, default(IEditorWindowDestroySystem));
			if (isShow)
			{
				isShow = false;
				entity?.Dispose();
			}
		}
	}
}

