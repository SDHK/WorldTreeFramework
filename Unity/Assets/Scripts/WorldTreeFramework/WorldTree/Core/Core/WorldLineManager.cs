/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/18 9:35

* 描述： 世界线程
*

*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace WorldTree
{

	/// <summary>
	/// 世界线管理器
	/// </summary>
	public class WorldLineManager : IDisposable
	{
		/// <summary>
		/// 日志类型
		/// </summary>
		public Type LogType;

		/// <summary>
		/// 日志等级
		/// </summary>
		public LogLevel LogLevel;

		/// <summary>
		/// 世界线集合
		/// </summary>
		private ConcurrentDictionary<int, WorldLine> lineDict = new();

		/// <summary>
		/// 主世界线
		/// </summary>
		private WorldLine mainLine;


		/// <summary>
		/// 可视化世界线
		/// </summary>
		private WorldLine viewLine;

		/// <summary>
		/// 可视化世界之心类型
		/// </summary>
		private Type viewHeartType;

		/// <summary>
		/// 可视化生成器类型
		/// </summary>
		private Type viewBuilderType;

		/// <summary>
		/// 设置可视化
		/// </summary>
		public void SetView(Type heartType, Type viewBuilderType)
		{
			viewHeartType = heartType;
			this.viewBuilderType = viewBuilderType;
		}

		/// <summary>
		/// 创建世界线
		/// </summary>
		public void Create(int id, Type heartType, int frameTime)
		{
			if (!lineDict.TryGetValue(id, out WorldLine line))
			{
				line = new WorldLine();
				line.WorldLineManager = this;
				line.Init(heartType, frameTime);
				lineDict.TryAdd(id, line);
			}
			else
			{
				throw new Exception($"世界线 {id} 已存在！");
			}
		}

		/// <summary>
		/// 销毁世界线
		/// </summary>
		public void Destroy(int id)
		{
			Get(id)?.WorldContext.Post(() =>
			{
				if (lineDict.Remove(id, out WorldLine line)) line.Dispose();
			});
		}

		/// <summary>
		/// 销毁所有世界线
		/// </summary>
		public void DestroyAll()
		{
			foreach (var line in lineDict.Values)
			{
				if (mainLine == line) continue;
				line.WorldContext.Post(() =>
				{
					line.Dispose();
				});
			}
			lineDict.Clear();

			mainLine?.WorldContext.Post(() =>
			{
				mainLine.Dispose();
			});

			mainLine = null;
		}

		/// <summary>
		/// 获取世界线
		/// </summary>
		public WorldLine Get(int id)
		{
			lineDict.TryGetValue(id, out WorldLine line);
			return line;
		}


		/// <summary>
		/// 释放
		/// </summary>
		public void Dispose()
		{
			DestroyAll();
		}
	}

}