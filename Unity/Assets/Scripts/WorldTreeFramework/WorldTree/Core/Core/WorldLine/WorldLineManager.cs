/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/18 9:35

* 描述： 世界线程管理器
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
		/// 启动选项
		/// </summary>
		public Options Options;

		/// <summary>
		/// 日志类型
		/// </summary>
		public Type LogType;

		/// <summary>
		/// 主世界线
		/// </summary>
		public WorldLine MainLine;

		/// <summary>
		/// 世界线集合
		/// </summary>
		private ConcurrentDictionary<int, WorldLine> lineDict = new();

		#region 可视化

		/// <summary>
		/// 可视化世界线
		/// </summary>
		public WorldLine ViewLine;

		/// <summary>
		/// 可视化世界之心类型
		/// </summary>
		private Type viewHeartType;

		/// <summary>
		/// 可视化生成器类型
		/// </summary>
		private Type viewBuilderType;

		/// <summary>
		/// 外部主线程更新方法
		/// </summary>
		public Action<TimeSpan> MainUpdate;

		/// <summary>
		/// 设置可视化
		/// </summary>
		public void SetView(Type worldType, Type heartType, Type viewBuilderType)
		{
			this.viewHeartType = heartType;
			this.viewBuilderType = viewBuilderType;
			ViewLine = new WorldLine();
			ViewLine.WorldLineManager = this;
			ViewLine.Init(heartType, 10);
			ViewLine.WorldContext.Post(() =>
			{
				long worldTypeCode = ViewLine.TypeToCode(worldType);
				NodeBranchHelper.AddNode(ViewLine, default(ComponentBranch), worldTypeCode, worldTypeCode, out _);
			});
		}

		#endregion

		/// <summary>
		/// 创建世界线
		/// </summary>
		public WorldLine Create(int id, Type heartType, int frameTime = 0)
		{
			if (!lineDict.TryGetValue(id, out WorldLine line))
			{
				line = new WorldLine();

				if (ViewLine != null)
				{
					//可视化世界线添加可视化生成器
					INode nodeView = ViewLine.PoolGetNode(ViewLine.TypeToCode(viewBuilderType));
					line.ViewBuilder = NodeBranchHelper.AddNodeToTree(ViewLine.World, default(ChildBranch), nodeView.Id, nodeView, (INode)line, default(INode)) as IWorldTreeNodeViewBuilder;
				}

				MainLine ??= line;
				line.WorldLineManager = this;
				line.Init(heartType, frameTime);
				lineDict.TryAdd(id, line);
				return line;
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
				if (MainLine == line) continue;
				line.WorldContext.Post(() =>
				{
					line.Dispose();
				});
			}
			lineDict.Clear();

			MainLine?.WorldContext.Post(() =>
			{
				MainLine.Dispose();
			});

			ViewLine?.WorldContext.Post(() =>
			{
				ViewLine.Dispose();
			});

			MainLine = null;
			ViewLine = null;
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