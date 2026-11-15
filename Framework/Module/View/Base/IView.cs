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
	public interface IView : INode
	{
		/// <summary>
		/// 视图名称（默认类名） 
		/// </summary>
		public string ViewName { get; set; }
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
