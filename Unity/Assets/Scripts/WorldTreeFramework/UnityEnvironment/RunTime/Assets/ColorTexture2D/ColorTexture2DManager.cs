/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// 颜色图片管理器帮助类
	/// </summary>
	public static partial class ColorTexture2DManagerHelper
	{
		/// <summary>
		/// 获取颜色图片
		/// </summary>
		public static Texture2D GetColorTexture(World self, Color color)
		{
			return self.AddComponent(out ColorTexture2DManager _).Get(color);
		}

		/// <summary>
		/// 获取黑白图片
		/// </summary>
		public static Texture2D GetColorTexture(World self, float color, float alpha = 1)
		{
			return self.AddComponent(out ColorTexture2DManager _).Get(new Color(color, color, color, alpha));
		}
	}

	/// <summary>
	/// 颜色图片管理器
	/// </summary>
	public class ColorTexture2DManager : Node, ComponentOf<World>
		, AsRule<Awake>
	{
		/// <summary>
		/// 颜色图片字典
		/// </summary>
		UnitDictionary<Color, Texture2D> colorDict = new UnitDictionary<Color, Texture2D>();
		/// <summary>
		/// 获取
		/// </summary>
		public Texture2D Get(Color color)
		{
			Texture2D texture = null;
			if (!colorDict.TryGetValue(color, out texture))
			{
				texture = new Texture2D(1, 1, Texture2D.grayTexture.format, true);
				texture.SetPixel(0, 0, color);

				colorDict.Add(color, texture);
				texture.Apply();
			}
			return texture;
		}
	}
}
