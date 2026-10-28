/****************************************

* 作者：闪电黑客
* 日期：2024/11/25 20:43

* 描述：

*/
using System;
using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 数据库管理器
	/// </summary>
	public abstract class DataBaseManager : Node
		, AsFieldBranch
		, ComponentOf<World>
	{
		/// <summary>
		/// 默认数据库分组
		/// </summary>
		public const string DefaultGroup = "";

		/// <summary>
		/// 分组到数据库来源
		/// </summary>
		private readonly Dictionary<string, IDataBase> sourceDict = new();

		/// <summary>
		/// 默认数据库来源
		/// </summary>
		public IDataBase Source => TryGetSource(DefaultGroup, out IDataBase source) ? source : null;

		/// <summary>
		/// 按分组获取数据库来源
		/// </summary>
		public IDataBase GetSource(string group = DefaultGroup)
		{
			string key = group ?? DefaultGroup;
			if (sourceDict.TryGetValue(key, out IDataBase source)) return source;

			throw new Exception($"数据库不存在: {key}");
		}

		/// <summary>
		/// 尝试按分组获取数据库来源
		/// </summary>
		public bool TryGetSource(string group, out IDataBase source)
		{
			return sourceDict.TryGetValue(group ?? DefaultGroup, out source);
		}

		/// <summary>
		/// 添加数据库来源；分组重复会报错
		/// </summary>
		public void AddSource(string group, IDataBase source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			string key = group ?? DefaultGroup;
			if (sourceDict.ContainsKey(key))
				throw new Exception($"数据库名重复: {key}");

			sourceDict.Add(key, source);
		}

		public override void OnDispose()
		{
			sourceDict.Clear();
			base.OnDispose();
		}
	}
}
