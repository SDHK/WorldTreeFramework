/****************************************

* 作者： 闪电黑客
* 日期： 2025/10/16

* 描述： 视图接口。

*/

namespace WorldTree
{
	/// <summary>
	/// 视图接口
	/// </summary>
	public interface IView
	{
		/// <summary>
		/// 视图打开
		/// </summary>
		public TreeTask Open();
		/// <summary>
		/// 视图关闭
		/// </summary>
		public TreeTask Close();

		/// <summary>
		/// 执行打开
		/// </summary>
		public TreeTask OnOpen();
		/// <summary>
		/// 执行关闭
		/// </summary>
		public TreeTask OnClose();

		/// <summary>
		/// 视图层级改变 
		/// </summary>
		public void LayerChange();
	}
}
