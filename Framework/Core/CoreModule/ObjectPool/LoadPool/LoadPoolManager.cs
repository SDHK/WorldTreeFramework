using System;
using System.Collections.Concurrent;

namespace WorldTree
{

	/// <summary>
	/// 加载对象池管理器基类
	/// </summary>
	public class LoadPoolManager : Node
		, ComponentOf<World>
		, AsChildBranch
		, AsRule<Awake>
	{
		/// <summary>
		/// 池集合字典
		/// </summary>
		public ConcurrentDictionary<string, LoadPool> PoolDict = new ConcurrentDictionary<string, LoadPool>();

		/// <summary>
		/// 新建池
		/// </summary>
		protected LoadPool NewPool(string loadPath, Type type)
		{
			if (!typeof(ILoadUnit).IsAssignableFrom(type))
				throw new InvalidOperationException($"{type} 必须实现 ILoadUnit");

			this.TypeToCode(type);
			//组合类型为 LoadPool<T>，添加到树上
			Type loadPoolType = typeof(LoadPool<>).MakeGenericType(type);
			INode node = this.World.PoolGetNode(loadPoolType);
			LoadPool pool = (LoadPool)NodeBranchHelper.AddNodeToTree(this, default(ChildBranch), node.Id, node);

			pool.ObjectType = type;
			pool.LoadPath = loadPath;
			return pool;
		}

		/// <summary>
		/// 按路径取得池。已有池的类型和请求不一致时抛出。
		/// </summary>
		protected LoadPool GetOrNewPool(Type type, string loadPath)
		{
			LoadPool pool = PoolDict.GetOrAdd(loadPath, NewPool, type);
			if (pool.ObjectType != type)
				throw new InvalidOperationException($"加载池 {loadPath} 已绑定 {pool.ObjectType}，不能作为 {type} 获取");
			return pool;
		}

		/// <summary>
		/// 按路径取得池。已有池的类型和请求不一致时抛出。
		/// </summary>
		protected LoadPool<Obj> GetOrNewPool<Obj>(string loadPath)
			where Obj : class, ILoadUnit
			=> (LoadPool<Obj>)GetOrNewPool(typeof(Obj), loadPath);

		/// <summary>
		/// 借出外壳。直接等待，或先接住任务再等待。
		/// </summary>
		public async TreeTask<Obj> Get<Obj>(string loadPath)
			where Obj : class, ILoadUnit
		{
			return await GetOrNewPool<Obj>(loadPath).GetObject();
		}

		/// <summary>
		/// 借出外壳。直接等待，或先接住任务再等待。
		/// </summary>
		public async TreeTask<ILoadUnit> Get(string loadPath, Type type)
		{
			return await GetOrNewPool(type, loadPath).GetObject();
		}

		/// <summary>
		/// 尝试获取对象池
		/// </summary>
		public virtual bool TryGetPool(string loadPath, out LoadPool pool)
		{
			return PoolDict.TryGetValue(loadPath, out pool);
		}

		/// <summary>
		/// 尝试回收对象
		/// </summary>
		public bool TryRecycle(ILoadUnit obj)
		{
			if (obj != null && TryGetPool(obj.LoadPath, out LoadPool pool))
			{
				pool.Recycle(obj);
				return true;
			}
			return false;
		}

		/// <summary>
		/// 释放池
		/// </summary>
		public virtual void DisposePool(string loadPath)
		{
			if (PoolDict.TryRemove(loadPath, out LoadPool pool)) pool.Dispose();
		}


		class Remove : RemoveRule<LoadPoolManager>
		{
			protected override void Execute(LoadPoolManager self)
			{
				foreach (var pool in self.PoolDict.Values) pool.Dispose();
				self.PoolDict.Clear();
			}
		}
	}
}


