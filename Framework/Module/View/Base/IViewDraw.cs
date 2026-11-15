/****************************************

* 作者： 闪电黑客
* 日期： 2025/10/16

* 描述： 视图绘制壳接口。

*/

namespace WorldTree
{
	/// <summary>
	/// 视图绘制壳：具体平台实现的显示对象外壳。入池加载或包住场景对象，放在 <see cref="View.Draw"/> 上。
	/// </summary>
	public interface IViewDraw : ILoadUnit
	{
		/// <summary>
		/// 是否激活显示
		/// </summary>
		bool IsActive { get; }

		/// <summary>
		/// 设置激活显示
		/// </summary>
		void SetActive(bool value);
	}


}
