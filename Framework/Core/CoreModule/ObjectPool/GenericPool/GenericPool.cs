
/******************************

 * 作者： 闪电黑客
 * 日期: 2021/12/13 13:37:00

 * 描述:  
 
    泛型通用对象池 ,继承 PoolBase
    
    需要手动注册New和Destroy的方法

    1.预加载数：objectPreload = 0 ，在 池新建时 和 Get时 , 将池内对象数量保持到设定数值。
    
    2.限制数：objectLimit = -1 ，为-1时则不限制对象数量。
        数量超过限制时，对象将不再被回收保留，而是被回收后立即销毁。

    设计思路是可以用于任意类型的对象管理，
    也可以被继承定义为指定对象管理的对象池。

*/
/****************************************

* 作者： 闪电黑客
* 日期： 2022/6/23 10:30

* 描述： 名称改为通用对象池，
* 去除了倒计时功能。

*/



using System;
using System.Collections.Concurrent;

namespace WorldTree
{

	/// <summary>
	/// 视图对象池 
	/// </summary>
	public abstract class ViewPool : Node
	{
		/// <summary>
		/// 当前保留对象数量
		/// </summary>
		public abstract int Count { get; }

		/// <summary>
		/// 类型 
		/// </summary>
		public Type ObjectType { get; protected set; }

		/// <summary>
		/// 约束数量
		/// </summary>
		public int MaxLimit;

		/// <summary>
		/// 预加载数量
		/// </summary>
		public int minLimit;

		/// <summary>
		/// 获取对象 
		/// </summary>
		public abstract object GetObject();
		/// <summary>
		/// 回收对象 
		/// </summary>
		public abstract void Recycle(object recycleObject);
		/// <summary>
		/// 释放一个
		/// </summary>
		public abstract void DisposeOne();
		/// <summary>
		/// 释放所有
		/// </summary>
		public abstract void DisposeAll();
		/// <summary>
		/// 预加载
		/// </summary>
		public abstract void Preload();
	}

	/// <summary>
	/// 泛型通用对象池
	/// </summary>
	public class GenericPool<T> : ViewPool
		where T : class
	{
		/// <summary>
		/// 对象池
		/// </summary>
		[Protected] public ConcurrentQueue<T> objectPoolQueue = new ConcurrentQueue<T>();

		/// <summary>
		/// 当前保留对象数量
		/// </summary>
		public override int Count => objectPoolQueue.Count;

		/// <summary>
		/// 实例化对象的方法
		/// </summary>
		public Func<ViewPool, T> NewObject;
		/// <summary>
		/// 销毁对象的方法
		/// </summary>
		public Action<T> DestroyObject;


		/// <summary>
		/// 对象新建时
		/// </summary>
		[Protected] public Action<T> objectOnNew;

		/// <summary>
		/// 对象获取时
		/// </summary>
		[Protected] public Action<T> objectOnGet;

		/// <summary>
		/// 对象回收时
		/// </summary>
		[Protected] public Action<T> objectOnRecycle;

		/// <summary>
		/// 对象销毁时
		/// </summary>
		[Protected] public Action<T> objectOnDestroy;


		//用于继承的子类可实现无参数构造函数
		protected GenericPool() { ObjectType = typeof(T); }

		/// <summary>
		/// 对象池构造 （实例化对象的委托，销毁对象的委托）
		/// </summary>
		public GenericPool(Func<ViewPool, T> objectNew, Action<T> objectDestroy = null)
		{
			ObjectType = typeof(T);
			this.NewObject = objectNew;
			this.DestroyObject = objectDestroy;
		}

		public override string ToString()
		{
			return "[GenericPool<" + ObjectType + ">]";
		}

		/// <summary>
		/// 从队列获取一个对象，假如队列无对象则新建对象
		/// </summary>
		public T DequeueOrNewObject()
		{
			T obj = null;
			if (objectPoolQueue.Count != 0)
			{
				objectPoolQueue.TryDequeue(out obj);
			}
			else
			{
				if (NewObject != null)
				{
					obj = NewObject(this);
					objectOnNew?.Invoke(obj);
				}
				return obj;
			}
			return obj;
		}


		/// <summary>
		/// 获取对象
		/// </summary>
		public virtual T Get()
		{

			T obj = DequeueOrNewObject();
			objectOnGet?.Invoke(obj);
			Preload();
			return obj;
		}


		public override object GetObject() => Get();


		public override void Recycle(object recycleObject)
		{
			if (recycleObject != null)
			{
				T obj = recycleObject as T;
				if (MaxLimit == -1 || objectPoolQueue.Count < MaxLimit)
				{
					//对象没有回收的标记。无法判断对象是否已经被回收。
					objectOnRecycle?.Invoke(obj);
					objectPoolQueue.Enqueue(obj);
				}
				else
				{
					objectOnRecycle?.Invoke(obj);
					objectOnDestroy?.Invoke(obj);
					DestroyObject?.Invoke(obj);
				}
			}
		}
		public override void DisposeOne()
		{
			if (objectPoolQueue.Count > 0)
			{
				if (objectPoolQueue.TryDequeue(out T obj))
				{
					objectOnDestroy?.Invoke(obj);
					DestroyObject?.Invoke(obj);
				}
			}
		}
		public override void DisposeAll()
		{
			while (objectPoolQueue.Count > 0)
			{
				if (objectPoolQueue.TryDequeue(out T obj))
				{
					objectOnDestroy?.Invoke(obj);
					DestroyObject?.Invoke(obj);
				}
			}
		}

		public override void Preload()
		{
			while (objectPoolQueue.Count < minLimit)
			{
				T obj = NewObject(this);
				objectOnNew?.Invoke(obj);
				objectPoolQueue.Enqueue(obj);
			}
		}
	}

	class GenericPoolRemoveRule<T> : RemoveRule<GenericPool<T>>
		where T : class
	{
		protected override void Execute(GenericPool<T> self)
		{
			self.DisposeAll();
			self.NewObject = null;
			self.DestroyObject = null;
			self.objectOnNew = null;
			self.objectOnGet = null;
			self.objectOnRecycle = null;
			self.objectOnDestroy = null;
		}
	}
}