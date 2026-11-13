using System;

namespace WorldTree
{
	public static partial class WorldRule
	{
		/// <summary>
		/// 获取加载池管理器。挂在核心主世界线的 MainWorld 上，没有就建一个。
		/// </summary>
		public static LoadPoolManager GetLoadPoolManager(this World self)
		{
			World owner = self.Line?.Core?.MainLine?.MainWorld;
			if (owner == null)
				throw new InvalidOperationException("核心主世界线的 MainWorld 不存在");

			if (owner.TryGetComponent(out LoadPoolManager manager)) return manager;
			owner.AddComponent(out manager);
			return manager;
		}

		/// <summary>
		/// 从加载池借出外壳。直接等待，或先接住任务再等待。
		/// </summary>
		public static async TreeTask<T> PoolGetLoad<T>(this World self, string loadPath)
			where T : class, ILoadUnit
		{
			return await self.GetLoadPoolManager().Get<T>(loadPath);
		}

		/// <summary>
		/// 从加载池借出外壳。直接等待，或先接住任务再等待。
		/// </summary>
		public static async TreeTask<ILoadUnit> PoolGetLoad(this World self, string loadPath, Type type)
		{
			return await self.GetLoadPoolManager().Get(loadPath, type);
		}

		/// <summary>
		/// 把外壳还回加载池。池已经不在时返回 false。
		/// </summary>
		public static bool PoolRecycleLoad(this World self, ILoadUnit obj)
		{
			if (obj == null) return false;
			return self.GetLoadPoolManager().TryRecycle(obj);
		}

	}
}
