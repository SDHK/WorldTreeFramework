/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// GUIBeginVertical
	/// </summary>
	public class GUIBeginVertical : GUIBase
	{
		/// <summary>
		/// 绘制
		/// </summary>
		public void Draw()
		{
			GUILayout.BeginVertical(Style, options);
		}
	}

	class GUIBeginVerticalAddSystem : AddRule<GUIBeginVertical>
	{
		protected override void Execute(GUIBeginVertical self)
		{
			self.Texture = ColorTexture2DManagerHelper.GetColorTexture(self.World, 0.1f);

			self.Padding = new RectOffset(5, 5, 5, 5);
		}
	}

	class GUIBeginVerticalRemoveSystem : RemoveRule<GUIBeginVertical>
	{
		protected override void Execute(GUIBeginVertical self)
		{
			//self.PoolRecycle(self.style);
			self.style = null;
		}
	}
}
