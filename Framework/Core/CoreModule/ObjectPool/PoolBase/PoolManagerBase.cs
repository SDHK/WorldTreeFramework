using System;
using System.Collections.Concurrent;

namespace WorldTree
{
	/// <summary>
	/// 对象池管理器泛型基类
	/// </summary>
	public abstract class PoolManagerBase<T> : CoreObject
		where T : PoolBase, new()
	{
		/// <summary>
		/// 忽略类型名单
		/// </summary>
		public ConcurrentDictionary<long, bool> IgnoreTypeHashDict = new();

		/// <summary>
		/// 池集合字典
		/// </summary>
		public ConcurrentDictionary<long, T> PoolDict = new ConcurrentDictionary<long, T>();

		/// <summary>
		/// 新建池委托
		/// </summary>
		private Func<long, Type, T> newPoolFunc;

		public override void OnCreate()
		{
			newPoolFunc = NewPool;
		}

		/// <summary>
		/// 新建池
		/// </summary>
		protected T NewPool(long typeCode, Type type)
		{
			T pool = new T();
			pool.Core = Core;
			pool.ObjectType = type;
			pool.ObjectTypeCode = Core.TypeInfo.TypeToCode(type);
			return pool;
		}

		/// <summary>
		/// 尝试新建或获取对象池
		/// </summary>
		protected virtual bool TryNewOrGetPool<Obj>(out T pool) => TryNewOrGetPool(typeof(Obj), TypeInfo<Obj>.Code, out pool);

		/// <summary>
		/// 尝试新建或获取对象池
		/// </summary>
		protected virtual bool TryNewOrGetPool(Type type, long typeCode, out T pool)
		{
			//忽略类型表检测
			if (!IgnoreTypeHashDict.ContainsKey(typeCode))
			{
				pool = PoolDict.GetOrAdd(typeCode, newPoolFunc, type);
				return true;
			}
			pool = null;
			return false;
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
			long typeCode = Core.TypeInfo.TypeToCode(type);
			if (TryNewOrGetPool(type, typeCode, out T pool))
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
		public bool TryRecycle(IBasic obj)
		{
			if (TryGetPool(obj.Type, out T pool))
			{
				pool.Recycle(obj);
				return true;
			}
			return false;
		}

		/// <summary>
		/// 尝试获取对象池
		/// </summary>
		public virtual bool TryGetPool(long type, out T pool)
		{
			return PoolDict.TryGetValue(type, out pool);
		}

		/// <summary>
		/// 释放池
		/// </summary>
		public virtual void DisposePool(long type)
		{
			if (PoolDict.TryRemove(type, out T pool)) pool.Dispose();
		}

		public override void OnDispose()
		{
			foreach (var pool in PoolDict.Values) pool.Dispose();
			PoolDict.Clear();
			IgnoreTypeHashDict.Clear();
		}
	}

}
