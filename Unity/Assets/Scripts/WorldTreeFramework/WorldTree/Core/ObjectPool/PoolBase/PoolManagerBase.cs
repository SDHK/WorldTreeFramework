using System;
using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 对象池管理器基类
	/// </summary>
	public abstract class PoolManagerBase : Node, IListenerIgnorer
		, AsRule<Awake>
	{
		/// <summary>
		/// 忽略类型名单
		/// </summary>
		public HashSet<long> ignoreTypeHash = new HashSet<long>();

		public override void Dispose()
		{
			NodeBranchHelper.RemoveNode(this);//从父节点分支移除
			IsDisposed = true;
			SetActive(false);
			base.Dispose();
		}
	}

	/// <summary>
	/// 对象池管理器泛型基类
	/// </summary>
	public abstract class PoolManagerBase<T> : PoolManagerBase
		where T : PoolBase
	{
		/// <summary>
		/// 池集合字典
		/// </summary>
		public Dictionary<long, T> poolDict = new Dictionary<long, T>();

		/// <summary>
		/// 尝试新建或获取对象池
		/// </summary>
		public virtual bool TryNewOrGetPool<Obj>(out T pool)
		{
			long typeCode = TypeInfo<Obj>.Code;
			//忽略类型表检测
			if (!ignoreTypeHash.Contains(typeCode))
			{
				//不存在则新建
				if (!poolDict.TryGetValue(typeCode, out pool))
				{
					pool = NewPool(typeof(Obj));
				}
				return true;
			}
			pool = null;
			return false;
		}

		/// <summary>
		/// 尝试新建或获取对象池：不安全
		/// </summary>
		public virtual bool TryNewOrGetPool(long type, out T pool)
		{
			//忽略类型表检测
			if (!ignoreTypeHash.Contains(type))
			{
				//不存在则新建
				if (!poolDict.TryGetValue(type, out pool))
				{
					pool = NewPool(Core.CodeToType(type));
				}
				return true;
			}
			pool = null;
			return false;
		}

		/// <summary>
		/// 新建池
		/// </summary>
		private T NewPool(Type type)
		{
			Core.NewNode(out T pool);
			pool.ObjectType = type;
			pool.ObjectTypeCode = this.TypeToCode(type);
			poolDict.Add(pool.ObjectTypeCode, pool);
			pool.TryGraftSelfToTree<ChildBranch, long>(pool.Id, this);
			pool.SetActive(true);
			return pool;
		}

		/// <summary>
		/// 尝试获取对象
		/// </summary>
		public bool TryGet<Obj>(out Obj obj)
			where Obj : class
		{
			if (TryNewOrGetPool<Obj>(out T pool))
			{
				obj = pool.GetObject() as Obj;
				return true;
			}
			else
			{
				obj = null;
				return false;
			}
		}

		/// <summary>
		/// 尝试获取对象
		/// </summary>
		public bool TryGet(Type type, out object obj)
		{
			long typeCode = this.TypeToCode(type);
			if (TryNewOrGetPool(typeCode, out T pool))
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
