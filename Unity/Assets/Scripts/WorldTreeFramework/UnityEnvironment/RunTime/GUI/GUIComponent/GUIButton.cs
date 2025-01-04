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
	/// GUIButton
	/// </summary>
	public class GUIButton : GUIBase
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
            if (GUILayout.Button(text, Style, options))
            {
                action?.Invoke();
            }
        }
    }

    class GUIButtonRemoveRule : RemoveRule<GUIButton>
    {
        protected override void Execute(GUIButton self)
        {
            //self.PoolRecycle(self.style);
            self.style = null;
        }
    }
}
