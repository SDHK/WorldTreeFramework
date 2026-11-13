namespace WorldTree
{
	/// <summary>
	/// 加载对象外壳。池里借出和回收的是它。
	/// 具体壳是 LoadUnit：未知类型放在 Target 里持有。由 LoadPool 编号，不走 PoolGetUnit。
	/// </summary>
	public interface ILoadUnit : IUnit
	{
		/// <summary>
		/// 加载路径，回收时用来找到池
		/// </summary>
		string LoadPath { get; set; }
	}

	/// <summary>
	/// 加载对象壳。继承单位，只负责持有未知对象。
	/// 编号和借还由 LoadPool 处理，不走 PoolGetUnit。
	/// </summary>
	public class LoadUnit : Unit, ILoadUnit
	{
		/// <summary>
		/// 加载路径，回收时用来找到池
		/// </summary>
		public string LoadPath { get; set; }

		/// <summary>
		/// 持有的对象。平台类型放在这里，不实现本类。
		/// </summary>
		public object Target;

		/// <summary>
		/// 释放。还回 LoadPool，不进 UnitPool。
		/// </summary>
		public override void Dispose()
		{
			if (IsDisposed || InstanceId == 0) return;
			if (World != null && World.PoolRecycleLoad(this)) return;
			IsDisposed = true;
			Id = 0;
			InstanceId = 0;
			Target = null;
		}
	}
}
