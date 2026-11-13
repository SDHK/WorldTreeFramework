using System;
using System.Collections.Concurrent;
using System.Threading;

namespace WorldTree
{
	/// <summary>
	/// 加载对象池。借还逻辑在这里，泛型子类只提供外壳类型。
	/// </summary>
	public abstract class LoadPool : Node
		, ChildOf<LoadPoolManager>
	{
		/// <summary>
		/// 对象类型
		/// </summary>
		public Type ObjectType;

		/// <summary>
		/// 加载路径
		/// </summary>
		public string LoadPath;

		/// <summary>
		/// 原型。资源或父级模板，不是借出的外壳。
		/// </summary>
		public object CopyObject;

		/// <summary>
		/// 当前对象数量
		/// </summary>
		public int Count => objectCount;

		/// <summary>
		/// 最小对象数量限制
		/// </summary>
		public int MinLimit = -1;

		/// <summary>
		/// 对象回收数量限制
		/// </summary>
		public int MaxLimit = -1;

		/// <summary>
		/// 热对象
		/// </summary>
		public ILoadUnit HotObject;

		/// <summary>
		/// 对象数量
		/// </summary>
		protected int objectCount;

		/// <summary>
		/// 对象池
		/// </summary>
		protected ConcurrentQueue<ILoadUnit> objectPoolQueue = new ConcurrentQueue<ILoadUnit>();

		/// <summary>
		/// 对象新建委托
		/// </summary>
		public Func<LoadPool, TreeTask<ILoadUnit>> NewObject;
		/// <summary>
		/// 销毁对象的方法
		/// </summary>
		public Action<LoadPool, ILoadUnit> DestroyObject;
		/// <summary>
		/// 对象新建时委托
		/// </summary>
		public Action<LoadPool, ILoadUnit> ObjectOnNew;
		/// <summary>
		/// 对象获取时委托
		/// </summary>
		public Action<LoadPool, ILoadUnit> ObjectOnGet;
		/// <summary>
		/// 对象回收时委托
		/// </summary>
		public Action<LoadPool, ILoadUnit> ObjectOnRecycle;
		/// <summary>
		/// 对象释放时委托
		/// </summary>
		public Action<LoadPool, ILoadUnit> ObjectOnDestroy;

		/// <summary>
		/// 获取对象
		/// </summary>
		public async TreeTask<ILoadUnit> GetObject()
		{
			ILoadUnit obj = HotObject;
			if (obj != null && Interlocked.CompareExchange(ref HotObject, null, obj) == obj)
			{
				await this.TreeTaskCompleted();
			}
			else if (objectPoolQueue.TryDequeue(out obj))
			{
				await this.TreeTaskCompleted();
				Interlocked.Decrement(ref objectCount);
			}
			else
			{
				obj = await NewObject(this);
				ObjectOnNew?.Invoke(this, obj);
			}

			Checkout(obj);
			TreeTaskToken token = await this.TreeTaskTokenCatch();
			if (token != null && (token.IsDisposed || token.State == TokenState.Cancel))
			{
				Recycle(obj);
				return null;
			}
			ObjectOnGet?.Invoke(this, obj);
			return obj;
		}

		/// <summary>
		/// 回收对象。类型不符时抛出。
		/// </summary>
		public void Recycle(ILoadUnit obj)
		{
			if (obj == null || obj.IsDisposed || obj.InstanceId == 0) return;
			if (ObjectType != null && !ObjectType.IsInstanceOfType(obj))
				throw new InvalidOperationException($"加载池 {LoadPath} 的类型是 {ObjectType}，不能回收 {obj.GetType()}");
			// 先失活再公开，避免热槽被取走时还没走过回收委托
			ObjectOnRecycle?.Invoke(this, obj);
			Park(obj);
			if (HotObject != null || Interlocked.CompareExchange(ref HotObject, obj, null) != null)
			{
				// 数量+1，假如超过限制则数量-1并丢弃对象，否则入队
				if (Interlocked.Increment(ref objectCount) <= MaxLimit || MaxLimit == -1)
				{
					objectPoolQueue.Enqueue(obj);
				}
				else
				{
					Interlocked.Decrement(ref objectCount);
					Destroy(obj);
				}
			}
		}

		/// <summary>
		/// 预加载
		/// </summary>
		public async TreeTask Preload()
		{
			while (objectPoolQueue.Count < MinLimit)
			{
				ILoadUnit obj = await NewObject(this);
				ObjectOnNew?.Invoke(this, obj);
				Park(obj);
				Interlocked.Increment(ref objectCount);
				objectPoolQueue.Enqueue(obj);
			}
			await this.TreeTaskCompleted();
		}

		/// <summary>
		/// 释放全部对象
		/// </summary>
		public void DisposeAll()
		{
			ILoadUnit hot = Interlocked.Exchange(ref HotObject, null);
			if (hot != null) Destroy(hot);
			while (objectPoolQueue.TryDequeue(out ILoadUnit obj))
			{
				Interlocked.Decrement(ref objectCount);
				Destroy(obj);
			}
		}

		/// <summary>
		/// 释放一个对象
		/// </summary>
		public void DisposeOne()
		{
			ILoadUnit obj = HotObject;
			if (obj != null && Interlocked.CompareExchange(ref HotObject, null, obj) == obj)
			{
				Destroy(obj);
				return;
			}
			if (objectPoolQueue.TryDequeue(out obj))
			{
				Interlocked.Decrement(ref objectCount);
				Destroy(obj);
			}
		}

		/// <summary>
		/// 借出时换一次实例号。不走 PoolGetUnit。
		/// </summary>
		private void Checkout(ILoadUnit obj)
		{
			obj.World = this.World;
			obj.LoadPath = this.LoadPath;
			obj.IsFromPool = true;
			obj.IsDisposed = false;
			long id = this.World.Line.Core.IdManager.GetId();
			obj.InstanceId = id;
			obj.Id = id;
		}

		/// <summary>
		/// 还池后作废这次借出。
		/// </summary>
		private static void Park(ILoadUnit obj)
		{
			obj.Id = 0;
			obj.InstanceId = 0;
		}

		/// <summary>
		/// 销毁一个池内对象
		/// </summary>
		private void Destroy(ILoadUnit obj)
		{
			obj.IsDisposed = true;
			Park(obj);
			ObjectOnDestroy?.Invoke(this, obj);
			DestroyObject?.Invoke(this, obj);
		}

		/// <inheritdoc/>
		public override void OnDispose()
		{
			DisposeAll();
		}

		class Add : AddRule<LoadPool>
		{
			protected override void Execute(LoadPool self)
			{
				self.LogError($"{self} 加载对象池没有对应初始化策略");
			}
		}
	}

	/// <summary>
	/// 按外壳类型区分的加载池。借还逻辑在基类。
	/// </summary>
	public class LoadPool<T> : LoadPool
		where T : class, ILoadUnit
	{
		/// <summary>
		/// 借出外壳。任务结果就是 T。
		/// </summary>
		public new async TreeTask<T> GetObject()
		{
			return (T)await base.GetObject();
		}
	}
}
