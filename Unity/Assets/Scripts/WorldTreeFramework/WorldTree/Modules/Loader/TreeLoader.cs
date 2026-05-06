/****************************************

* 作者： 闪电黑客
* 日期： 2025/11/17 11:54

* 描述： 

*/
namespace WorldTree
{
	//加载器，GameObjcet池.

	/// <summary>
	/// 树加载器
	/// </summary>
	public class TreeLoader : Node
	{
		/// <summary>
		/// 等待加载视图 ??? 事件传递？
		/// </summary>
		public TreeLoader Wait;
		//public NodeRef<INode> Wait;

		/// <summary>
		/// 目标视图绑定
		/// </summary>
		public NodeRef<INode> target;

		/// <summary>
		/// 对象池 
		/// </summary>
		public PoolBase pool;
	}
}
