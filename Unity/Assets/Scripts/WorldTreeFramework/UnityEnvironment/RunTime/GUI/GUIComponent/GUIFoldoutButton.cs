/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/29 21:26

* 描述： 

*/
using System;
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// GUI折叠按钮
	/// </summary>
	public class GUIFoldoutButton : GUIBase
    {
		/// <summary>
		/// 回调
		/// </summary>
		public Action action;

		/// <summary>
		/// 是否折叠
		/// </summary>
		public bool isFoldout;

		/// <summary>
		/// 绘制
		/// </summary>
		public bool Draw()
        {
            if (GUILayout.Button(text, Style, options))
            {
                isFoldout = !isFoldout;
                action?.Invoke();
            }
            return isFoldout;
        }

		/// <summary>
		/// 绘制
		/// </summary>
		public bool Draw(bool value)
        {
            this.isFoldout = value;
            if (GUILayout.Button(text, Style, options))
            {
                isFoldout = !isFoldout;
                action?.Invoke();
            }
            return isFoldout;
        }
    }
    class GUIFoldoutButtonRecycleSystem : RecycleRule<GUIFoldoutButton>
    {
        protected override void Execute(GUIFoldoutButton self)
        {
            //self.PoolRecycle(self.style);
            self.style = null;
        }
    }
}
