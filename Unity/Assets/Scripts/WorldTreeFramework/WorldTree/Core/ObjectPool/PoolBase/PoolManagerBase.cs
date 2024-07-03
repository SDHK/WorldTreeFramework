using System.Collections.Generic;
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
		public HashSet<long> ignoreTypeHash = new HashSet<long>();

		public override void Dispose()
		{
			NodeBranchHelper.RemoveBranchNode(Parent, BranchType, this);//从父节点分支移除
			IsDisposed = true;
			SetActive(false);
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
		public Dictionary<long, T> poolDict = new Dictionary<long, T>();

		/// <summary>
		/// 尝试新建或获取对象池
		/// </summary>
		public virtual bool TryNewOrGetPool(long type, out T pool)
		{
			//忽略类型表检测
			if (!ignoreTypeHash.Contains(type))
			{
				//不存在则新建
				if (!poolDict.TryGetValue(type, out pool))
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
			Core.NewNodeLifecycle(out T pool);
			pool.ObjectType = type.CodeToType();
			pool.ObjectTypeCode = type;
			poolDict.Add(pool.ObjectTypeCode, pool);
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
		public bool TryRecycle(IWorldTreeBasic obj)
		{
			if (TryNewOrGetPool(obj.Type, out T pool))
			{
				pool.Recycle(obj);
				return true;
			}
			else if (obj is T Tpool)
			{
				poolDict.Remove(Tpool.ObjectTypeCode);
			}
			return false;
		}

		/// <summary>
		/// 尝试获取对象池
		/// </summary>
		public virtual bool TryGetPool(long type, out T pool)
		{
			return poolDict.TryGetValue(type, out pool);
		}

		/// <summary>
		/// 释放池
		/// </summary>
		public virtual void DisposePool(long type)
		{
			if (poolDict.TryGetValue(type, out T pool))
			{
				pool.Dispose();
			}
		}
	}

}
