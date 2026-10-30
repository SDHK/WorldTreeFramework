/****************************************

* 作者： 闪电黑客
* 日期： 2026/10/20 16:25

* 描述： 进程级配置管理器。表是普通对象，所有世界线共用。

*/
using System;
using System.Collections.Concurrent;

namespace WorldTree
{
	/// <summary>
	/// 配置管理器
	/// </summary>
	/// <remarks>挂在 <see cref="WorldTreeCore"/> 上，进程内一份。</remarks>
	public class ConfigManager : CoreObject
	{
		/// <summary>
		/// 已登记的表
		/// </summary>
		private readonly ConcurrentDictionary<Type, object> tableDict = new();

		/// <summary>
		/// 登记一张表。同类型再次登记会替换。
		/// </summary>
		public void Add<T>(T table)
			where T : class
		{
			if (table == null) throw new ArgumentNullException(nameof(table));
			tableDict[typeof(T)] = table;
		}

		/// <summary>
		/// 获取一张表
		/// </summary>
		public T Get<T>()
			where T : class
		{
			if (TryGet(out T table)) return table;
			throw new Exception($"配置表不存在: {typeof(T).Name}");
		}

		/// <summary>
		/// 尝试获取一张表
		/// </summary>
		public bool TryGet<T>(out T table)
			where T : class
		{
			if (tableDict.TryGetValue(typeof(T), out object value))
			{
				table = (T)value;
				return true;
			}
			table = null;
			return false;
		}

		/// <summary>
		/// 释放已登记的表
		/// </summary>
		public override void OnDispose()
		{
			tableDict.Clear();
		}
	}
}
