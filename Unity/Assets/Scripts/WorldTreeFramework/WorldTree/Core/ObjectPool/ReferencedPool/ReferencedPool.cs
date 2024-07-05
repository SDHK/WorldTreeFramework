/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/4 14:43

* 描述： 引用池管理器

*/

using System;
using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 引用池
	/// </summary>
	public class ReferencedPool : Node, IListenerIgnorer, ChildOf<ReferencedPoolManager>
		, AsComponentBranch
	{
		/// <summary>
		/// 节点字典
		/// </summary>
		private Dictionary<long, INode> nodeDict = new Dictionary<long, INode>();

		/// <summary>
		/// 节点字典
		/// </summary>
		public Dictionary<long, INode> NodeDict => nodeDict;

		/// <summary>
		/// 添加节点
		/// </summary>
		public bool TryAdd(long id, INode node)
		{
			return nodeDict.TryAdd(id, node);
		}

		/// <summary>
		/// 移除节点
		/// </summary>
		public void Remove(long id)
		{
			nodeDict.Remove(id);
		}


		/// <summary>
		/// 引用池类型
		/// </summary>
		public long ReferencedType;
		public override string ToString()
		{
			return $"ReferencedPool<{ReferencedType.CodeToType()}>:[{nodeDict.Count}]";
		}
	}
}
