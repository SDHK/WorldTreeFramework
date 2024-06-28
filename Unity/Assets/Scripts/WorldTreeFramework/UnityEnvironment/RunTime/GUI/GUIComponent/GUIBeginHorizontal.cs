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

    class GUIBeginHorizontalRecycleSystem : RecycleRule<GUIBeginHorizontal>
    {
        protected override void Execute(GUIBeginHorizontal self)
        {
            //self.PoolRecycle(self.style);
            self.style = null;
        }
    }
}
