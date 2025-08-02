﻿/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using UnityEngine;


//思考self.Log方法

//GUIDrawSystem绘制UI


namespace WorldTree
{

	/// <summary>
	/// GUIWindow
	/// </summary>
	public class GUIWindow : GUIBase
		, ComponentOf<GUIGeneralWindow>
		, AsRule<Awake>
		, AsRule<GuiUpdate>
	{
		/// <summary>
		/// 标题
		/// </summary>
		public Rect rect = new Rect(0, 0, 400, 300);
		/// <summary>
		/// 是否拖拽
		/// </summary>
		public bool isDrag = true;
		/// <summary>
		/// 绘制法则组
		/// </summary>
		public IRuleGroup<GUIDraw> group;

		/// <summary>
		/// 样式
		/// </summary>
		public void Window(int id)
		{
			group?.Send(Parent);
			if (isDrag)
			{
				GUI.DragWindow();
			}
		}
	}

	class GUIWindowAddSystem : AddRule<GUIWindow>
	{
		protected override void Execute(GUIWindow self)
		{
			//self.group = self.Core.RuleManager.GetRuleGroup<IGUIDrawSystem>();
		}
	}

	class GUIWindowOnGUISystem : GuiUpdateRule<GUIWindow>
	{
		protected override void Execute(GUIWindow self)
		{
			self.rect = GUILayout.Window(self.GetHashCode(), self.rect, self.Window, default(string), self.Style);
		}
	}

	class GUIWindowRemoveRule : RemoveRule<GUIWindow>
	{
		protected override void Execute(GUIWindow self)
		{
			//self.PoolRecycle(self.style);
			self.style = null;
		}
	}


}
