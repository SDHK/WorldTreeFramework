/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// GUIBeginHorizontal
	/// </summary>
	public class GUIBeginHorizontal : GUIBase
    {
		/// <summary>
		/// 绘制
		/// </summary>
		public void Draw()
        {
            GUILayout.BeginHorizontal(Style, options);
        }
    }

    class GUIBeginHorizontalRemoveRule : RemoveRule<GUIBeginHorizontal>
    {
        protected override void Execute(GUIBeginHorizontal self)
        {
            //self.PoolRecycle(self.style);
            self.style = null;
        }
    }
}
