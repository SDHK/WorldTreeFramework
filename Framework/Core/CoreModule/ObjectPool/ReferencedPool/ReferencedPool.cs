/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/4 14:43

* 描述： 引用池管理器

*/

using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 引用池
	/// </summary>
	public class ReferencedPool : Node, IListenerIgnorer
		, ChildOf<ReferencedPoolManager>
		, AsComponentBranch
	{
		/// <summary>
		/// 节点字典
		/// </summary>
		private Dictionary<long, INode> nodeDict = new Dictionary<long, INode>();
		/// <summary>
		/// 节点数据字典 
		/// </summary>
		private Dictionary<long, INodeData> nodeDataDict;

		/// <summary>
		/// 节点字典
		/// </summary>
		public Dictionary<long, INode> NodeDict => nodeDict;


		/// <summary>
		/// 尝试获取节点数据 
		/// </summary>
		public bool TryGetData(long uid, out INodeData nodeData)
		{
			nodeData = default;
			if (nodeDataDict == null) return false;
			if (!nodeDataDict.TryGetValue(uid, out nodeData)) return false;
			if (nodeData != null) return true;
			nodeDataDict.Remove(uid);
			return false;
		}

		/// <summary>
		/// 添加节点
		/// </summary>
		public bool TryAdd(long id, INode node)
		{
			if (node is INodeData nodeData)
			{
				nodeDataDict ??= new();
				nodeDataDict.TryAdd(node.Id, nodeData);
			}
			return nodeDict.TryAdd(id, node);
		}

		/// <summary>
		/// 移除节点
		/// </summary>
		public void Remove(long id)
		{
			nodeDataDict?.Remove(id);
			nodeDict.Remove(id);
		}

		/// <summary>
		/// 引用池类型
		/// </summary>
		public long ReferencedType;
		public override string ToString()
		{
			return $"ReferencedPool<{this.CodeToType(ReferencedType)}>:[{nodeDict.Count}]";
		}
	}
}
