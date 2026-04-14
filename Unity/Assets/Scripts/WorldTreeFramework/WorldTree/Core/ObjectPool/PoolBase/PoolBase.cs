/****************************************

* 作者： 闪电黑客
* 日期： 2026/4/13 14:25

* 描述： 多线程对象池基类

*/

using System;
using System.Collections.Concurrent;
using System.Threading;

namespace WorldTree
{
	/// <summary>
	/// 对象池抽象基类
	/// </summary>
	public abstract class PoolBase : CoreObjectBase
	{
		/// <summary>
		/// 对象类型
		/// </summary>
		public Type ObjectType;

		/// <summary>
		/// 对象类型码
		/// </summary>
		public long ObjectTypeCode;

		/// <summary>
		/// 当前对象数量
		/// </summary>
		public int Count => objectCount;

		/// <summary>
		/// 热对象 
		/// </summary>
		public object HotObject;

		/// <summary>
		/// 对象池
		/// </summary>
		protected ConcurrentQueue<object> objectPoolQueue = new ConcurrentQueue<object>();



		/// <summary>
		/// 对象数量 
		/// </summary>
		protected int objectCount;

		/// <summary>
		/// 对象回收数量限制
		/// </summary>
		public int MaxLimit = -1;

		/// <summary>
		/// 对象新建 
		/// </summary>
		protected virtual object NewObject()
		{
			return Activator.CreateInstance(ObjectType);
		}
		/// <summary>
		/// 获取对象
		/// </summary>
		public virtual object GetObject()
		{
			object obj = HotObject;
			if (obj == null || Interlocked.CompareExchange(ref HotObject, null, obj) != obj)
			{
				if (objectPoolQueue.TryDequeue(out obj))
				{
					//ConcurrentQueue 的 Count 属性不是 O(1)，它需要遍历内部所有分段累加
					Interlocked.Decrement(ref objectCount);
					return obj;
				}
				return NewObject();
			}
			return obj;
		}

		/// <summary>
		/// 回收对象
		/// </summary>
		public virtual void Recycle(object obj)
		{
			if (HotObject != null || Interlocked.CompareExchange(ref HotObject, obj, null) != null)
			{
				// 数量+1，假如超过限制则数量-1并丢弃对象，否则入队
				if (Interlocked.Increment(ref objectCount) <= MaxLimit || MaxLimit == -1)
				{
					objectPoolQueue.Enqueue(obj);
					return;
				}
				Interlocked.Decrement(ref objectCount);
			}
		}

		/// <summary>
		/// 释放全部对象
		/// </summary>
		public virtual void DisposeAll()
		{
			Interlocked.Exchange(ref HotObject, null);   // 直接清 HotObject
			while (objectPoolQueue.TryDequeue(out _))    // 逐一清队列
				Interlocked.Decrement(ref objectCount);
		}

		/// <summary>
		/// 释放一个对象
		/// </summary>
		public virtual void DisposeOne()
		{
			object obj = HotObject;
			if (obj == null || Interlocked.CompareExchange(ref HotObject, null, obj) != obj)
			{
				if (objectPoolQueue.TryDequeue(out _))
					Interlocked.Decrement(ref objectCount);
			}
		}

		public override void OnDispose()
		{
			DisposeAll();
		}
	}
}