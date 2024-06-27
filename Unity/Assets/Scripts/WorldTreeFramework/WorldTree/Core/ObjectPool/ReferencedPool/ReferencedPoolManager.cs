/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/4 14:41

* 描述： 引用池管理器
* 
* 有些类型是没有用对象池生成的，
* 所以将引用池从对象池里分离了出来。
* 

*/

using WorldTree.Internal;

namespace WorldTree
{
	/// <summary>
	/// 引用池管理器
	/// </summary>
	public class ReferencedPoolManager : Node, IListenerIgnorer, ComponentOf<WorldTreeCore>
	{
		/// <summary>
		/// 全部节点
		/// </summary>
		public UnitDictionary<long, INode> allNodeDict = new UnitDictionary<long, INode>();
		/// <summary>
		/// 分类节点
		/// </summary>
		public UnitDictionary<long, ReferencedPool> poolDict = new UnitDictionary<long, ReferencedPool>();

		public override void OnDispose()
		{
			NodeBranchHelper.RemoveBranchNode(this.Parent, this.BranchType, this);//从父节点分支移除
			allNodeDict.Clear();
			poolDict.Clear();
			allNodeDict = null;
			poolDict = null;
			this.IsRecycle = true;
			this.IsDisposed = true;
		}
	}


	public static class ReferencedPoolManagerRule
	{

		/// <summary>
		/// 添加节点对象
		/// </summary>
		public static bool TryAdd(this ReferencedPoolManager self, INode node)
		{
			self.allNodeDict.TryAdd(node.Id, node);
			return self.GetPool(node.Type).TryAdd(node.Id, node);
		}

		/// <summary>
		/// 移除节点对象
		/// </summary>
		public static void Remove(this ReferencedPoolManager self, INode node)
		{
			self.allNodeDict.Remove(node.Id);
			if (self.TryGetPool(node.Type, out ReferencedPool pool))
			{
				pool.Remove(node.Id);
			}
		}

		/// <summary>
		/// 获取池
		/// </summary>
		public static ReferencedPool GetPool(this ReferencedPoolManager self, long type)
		{
			if (!self.poolDict.TryGetValue(type, out ReferencedPool pool))
			{
				self.Core.NewNodeLifecycle(out pool);
				pool.ReferencedType = type.CodeToType();
				self.poolDict.Add(type, pool);
				pool.TryGraftSelfToTree<ChildBranch, long>(pool.Id, self);
				pool.SetActive(true);
			}
			return pool;
		}
		/// <summary>
		/// 尝试获取池
		/// </summary>
		public static bool TryGetPool(this ReferencedPoolManager self, long type, out ReferencedPool pool)
		{
			return self.poolDict.TryGetValue(type, out pool);
		}


	}
}
