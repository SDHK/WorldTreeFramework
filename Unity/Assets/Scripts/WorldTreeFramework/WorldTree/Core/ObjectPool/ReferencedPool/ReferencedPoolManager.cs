/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/4 14:41

* 描述： 引用池管理器
* 
* 有些类型是没有用对象池生成的，
* 所以将引用池从对象池里分离了出来。
* 

*/

namespace WorldTree
{
	/// <summary>
	/// 引用池管理器
	/// </summary>
	public class ReferencedPoolManager : Node, IListenerIgnorer
		, CoreManagerOf<WorldTreeCore>
	{
		/// <summary>
		/// 全部节点
		/// </summary>
		public UnitDictionary<long, INode> AllNodeDict = new UnitDictionary<long, INode>();

		/// <summary>
		/// 全部节点数据
		/// </summary>
		public UnitDictionary<long, INodeData> AllNodeDataDict = new UnitDictionary<long, INodeData>();

		/// <summary>
		/// 分类节点
		/// </summary>
		public UnitDictionary<long, ReferencedPool> poolDict = new UnitDictionary<long, ReferencedPool>();

		public override void OnDispose()
		{
			NodeBranchHelper.RemoveNode(this);//从父节点分支移除
			AllNodeDict.Clear();
			poolDict.Clear();
			AllNodeDict = null;
			poolDict = null;
			IsDisposed = true;
		}

		/// <summary>
		/// 添加节点对象
		/// </summary>
		public bool TryAdd(INode node)
		{
			AllNodeDict.TryAdd(node.InstanceId, node);
			if (node is INodeData nodeData)
			{

				AllNodeDataDict.TryAdd(node.Id, nodeData);
			}
			return GetPool(node.Type).TryAdd(node.InstanceId, node);
		}

		/// <summary>
		/// 移除节点对象
		/// </summary>
		public void Remove(INode node)
		{
			AllNodeDict.Remove(node.InstanceId);
			if (node is INodeData)
				AllNodeDataDict.Remove(node.Id);
			if (TryGetPool(node.Type, out ReferencedPool pool))
			{
				pool.Remove(node.InstanceId);
			}
		}

		/// <summary>
		/// 获取池
		/// </summary>
		public ReferencedPool GetPool(long type)
		{
			if (!poolDict.TryGetValue(type, out ReferencedPool pool))
			{
				Core.NewNode(out pool);
				pool.ReferencedType = type;
				poolDict.Add(type, pool);
				pool.TryGraftSelfToTree<ChildBranch, long>(pool.Id, this);
				pool.SetActive(true);
			}
			return pool;
		}
		/// <summary>
		/// 尝试获取池
		/// </summary>
		public bool TryGetPool(long type, out ReferencedPool pool)
		{
			return poolDict.TryGetValue(type, out pool);
		}
	}
}
