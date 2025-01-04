/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System;
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// Gui重复按钮
	/// </summary>
	public class GUIRepeatButton : GUIBase
    {
		/// <summary>
		/// 回调
		/// </summary>
		public Action action;

		/// <summary>
		/// 绘制
		/// </summary>
		public void Draw()
        {
            if (GUILayout.RepeatButton(text, Style, options))
            {
                action?.Invoke();
            }
        }
    }

    class GUIRepeatButtonRemoveRule : RemoveRule<GUIRepeatButton>
    {
        protected override void Execute(GUIRepeatButton self)
        {
            //self.PoolRecycle(self.style);
            self.style = null;
        }
    }
}
