using WorldTree.Internal;

namespace WorldTree
{
	/// <summary>
	/// 对象池管理器基类
	/// </summary>
	public abstract class PoolManagerBase : Node, IListenerIgnorer
	 , ComponentOf<WorldTreeCore>
	 , AsAwake

	{
		/// <summary>
		/// 忽略类型名单
		/// </summary>
		public TreeHashSet<long> m_IgnoreTypeHashSet = new TreeHashSet<long>();

		public override void Dispose()
		{
			this.Parent.RemoveBranchNode(this.BranchType, this);//从父节点分支移除
			this.IsRecycle = true;
			this.IsDisposed = true;
			this.SetActive(false);
			base.Dispose();
		}
	}

	/// <summary>
	/// 对象池管理器泛型基类
	/// </summary>
	public abstract class PoolManagerBase<T> : PoolManagerBase
		where T : PoolBase, ChildOf<PoolManagerBase<T>>
	{
		/// <summary>
		/// 池集合字典
		/// </summary>
		public TreeDictionary<long, T> m_Pools = new TreeDictionary<long, T>();


		/// <summary>
		/// 尝试新建或获取对象池
		/// </summary>
		public virtual bool TryNewOrGetPool(long type, out T pool)
		{
			//忽略类型表检测
			if (!m_IgnoreTypeHashSet.Contains(type))
			{
				//不存在则新建
				if (!m_Pools.TryGetValue(type, out pool))
				{
					pool = NewPool(type);
				}
				return true;
			}
			pool = null;
			return false;
		}

		/// <summary>
		/// 新建池
		/// </summary>
		private T NewPool(long type)
		{
			this.NewNodeLifecycle(out T pool);
			pool.ObjectType = type.CodeToType();
			pool.ObjectTypeCore = type;
			this.m_Pools.Add(pool.ObjectTypeCore, pool);
			pool.TryGraftSelfToTree<ChildBranch, long>(pool.Id, this);
			pool.SetActive(true);
			return pool;
		}

		/// <summary>
		/// 尝试获取对象
		/// </summary>
		public bool TryGet(long type, out object obj)
		{
			if (TryNewOrGetPool(type, out T pool))
			{
				obj = pool.GetObject();
				return true;
			}
			else
			{
				obj = null;
				return false;
			}
		}

		/// <summary>
		/// 尝试回收对象
		/// </summary>
		public bool TryRecycle(IUnit obj)
		{
			if (TryNewOrGetPool(obj.Type, out T pool))
			{
				pool.Recycle(obj);
				return true;
			}
			else if (obj is T Tpool)
			{
				m_Pools.Remove(Tpool.ObjectTypeCore);
			}
			return false;
		}



		/// <summary>
		/// 尝试获取对象池
		/// </summary>
		public virtual bool TryGetPool(long type, out T pool)
		{
			return m_Pools.TryGetValue(type, out pool);
		}

		/// <summary>
		/// 释放池
		/// </summary>
		public virtual void DisposePool(long type)
		{
			if (m_Pools.TryGetValue(type, out T pool))
			{
				pool.Dispose();
			}
		}
	}

	//class AddRule : AddRule<NodePoolManager>
	//{
	//    protected override void OnEvent(NodePoolManager self)
	//    {
	//        self.AddChild(out self.m_Pools);
	//    }
	//}

	//class RemoveRule : RemoveRule<NodePoolManager>
	//{
	//    protected override void OnEvent(NodePoolManager self)
	//    {
	//        self.m_Pools = null;
	//    }
	//}



}
