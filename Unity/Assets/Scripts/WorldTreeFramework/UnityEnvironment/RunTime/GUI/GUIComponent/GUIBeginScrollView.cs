/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// GUIBeginScrollView
	/// </summary>
	public class GUIBeginScrollView : GUIBase
    {
        /// <summary>
        /// 滚动位置
        /// </summary>
        public Vector2 scrollPosition;
        /// <summary>
        /// 绘制
        /// </summary>
        public void Draw()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, Style, options);
        }
    }

    class GUIBeginScrollViewRecycleSystem : RemoveRule<GUIBeginScrollView>
    {
        protected override void Execute(GUIBeginScrollView self)
        {
            //self.PoolRecycle(self.style);
            self.style = null;
        }
    }
}
