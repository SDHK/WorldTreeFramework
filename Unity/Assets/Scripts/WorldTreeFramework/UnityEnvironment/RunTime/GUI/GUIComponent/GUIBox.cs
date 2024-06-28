
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/29 16:17

* 描述： 

*/

using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// GUIBox
	/// </summary>
	public class GUIBox : GUIBase
    {
		/// <summary>
		/// 绘制
		/// </summary>
		public void Draw()
        {
            GUILayout.Box(text, Style, options);
        }
    }

    class GUIBoxRecycleSystem : RecycleRule<GUIBox>
    {
        protected override void Execute(GUIBox self)
        {
            //self.PoolRecycle(self.style);
            self.style = null;
        }
    }
}
