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
	public class WorldLineManager : CoreObjectBase
	{
		/// <summary>
		/// 启动选项
		/// </summary>
		public Options Options = new();

		/// <summary>
		/// 日志类型
		/// </summary>
		public Type LogType;

		/// <summary>
		/// 日志管理器
		/// </summary>
		public LogManager LogManager;

		/// <summary>
		/// 类型信息 
		/// </summary>
		public TypeInfo TypeInfo = new();

		/// <summary>
		/// 单位对象池管理器
		/// </summary>
		public UnitPoolManager UnitPoolManager;

		/// <summary>
		/// 节点对象池管理器
		/// </summary>
		public NodePoolManager NodePoolManager;

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


		public WorldLineManager()
		{
			Core = this;
			this.NewCoreObject(out LogManager);
			LogManager.Init("Core");
			this.NewCoreObject(out TypeInfo);
			this.NewCoreObject(out UnitPoolManager);
			this.NewCoreObject(out NodePoolManager);
		}

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
				NodeBranchHelper.AddNode(ViewLine, default(ComponentBranch), worldTypeCode, worldType, out _);
			});
		}

		#endregion

		/// <summary>
		/// 设置启动项
		/// </summary>
		public void SetOptions(Options options) => Options = options;

		/// <summary>
		/// 设置日志类型
		/// </summary>
		public void SetLog<T>() where T : class, ILog, new() => LogType = typeof(T);

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
					INode nodeView = ViewLine.PoolGetNode(viewBuilderType);
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
		public override void OnDispose()
		{
			DestroyAll();
		}
	}

	public static class WorldLineManagerRule
	{
		/// <summary>
		/// 新建核心对象
		/// </summary>
		public static CoreObjectBase NewCoreObject(this CoreObjectBase self, Type type)
		{
			CoreObjectBase obj = (CoreObjectBase)Activator.CreateInstance(type);
			obj.Core = self.Core;
			return obj;
		}

		/// <summary>
		/// 新建核心对象
		/// </summary>
		public static T NewCoreObject<T>(this CoreObjectBase self, out T newObj) where T : CoreObjectBase, new()
		{
			newObj = new T();
			newObj.Core = self.Core;
			return newObj;
		}

		/// <summary>
		/// 新建核心对象
		/// </summary>
		public static T NewCoreObject<T>(this CoreObjectBase self) where T : CoreObjectBase, new()
		{
			T newObj = new T();
			newObj.Core = self.Core;
			return newObj;
		}
	}
}