/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 16:21

* 描述： 动态数据队列
* 
* 主要为了可以按照顺序遍历的同时可随机移除内容

*/

namespace WorldTree
{
	/// <summary>
	/// 动态数据队列
	/// </summary>
	public class DynamicQueueNode : ZipperIterator<NodeRef<INode>>
	{

		public override bool CheckNull(NodeRef<INode> data)
		{
			return data.IsNull;
		}
	}
}
