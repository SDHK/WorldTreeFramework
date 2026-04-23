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
	/// 世界树核心
	/// </summary>
	public class WorldTreeCore : CoreObject
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
		/// 真实时间管理器
		/// </summary>
		public RealTimeManager RealTimeManager;

		/// <summary>
		/// 法则管理器
		/// </summary>
		public RuleManager RuleManager;


		/// <summary>
		/// Id管理器
		/// </summary>
		public IdManager IdManager;

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
		/// 可视化生成器类型
		/// </summary>
		private Type viewBuilderType;

		/// <summary>
		/// 外部主线程更新方法
		/// </summary>
		public Action<TimeSpan> MainUpdate;


		public WorldTreeCore()
		{
			Core = this;
			this.CreateCoreObject(out LogManager);
			LogManager.Init("Core");
			this.CreateCoreObject(out TypeInfo);
			this.CreateCoreObject(out RuleManager);
			this.CreateCoreObject(out RealTimeManager);
			this.CreateCoreObject(out IdManager);
			this.CreateCoreObject(out UnitPoolManager);
			this.CreateCoreObject(out NodePoolManager);


			AddRuleGroup = RuleManager.GetOrNewRuleGroup<Add>();
			RemoveRuleGroup = RuleManager.GetOrNewRuleGroup<Remove>();
			BeforeRemoveRuleGroup = RuleManager.GetOrNewRuleGroup<BeforeRemove>();
			EnableRuleGroup = RuleManager.GetOrNewRuleGroup<Enable>();
			DisableRuleGroup = RuleManager.GetOrNewRuleGroup<Disable>();
			GraftRuleGroup = RuleManager.GetOrNewRuleGroup<Graft>();
			CutRuleGroup = RuleManager.GetOrNewRuleGroup<Cut>();
			SerializeRuleGroup = RuleManager.GetOrNewRuleGroup<Serialize>();
			DeserializeRuleGroup = RuleManager.GetOrNewRuleGroup<Deserialize>();
		}

		/// <summary>
		/// 设置可视化
		/// </summary>
		public void SetView(Type worldType, Type heartType, Type viewBuilderType)
		{
			this.viewBuilderType = viewBuilderType;
			ViewLine = new WorldLine();
			ViewLine.WorldLineManager = this;
			ViewLine.Init(worldType, heartType, 10);
		}

		#endregion

		#region 基础生命周期

		/// <summary>
		/// 添加法则组
		/// </summary>
		public IRuleGroup<Add> AddRuleGroup;
		/// <summary>
		/// 移除前法则组
		/// </summary>
		public IRuleGroup<BeforeRemove> BeforeRemoveRuleGroup;
		/// <summary>
		/// 移除法则组
		/// </summary>
		public IRuleGroup<Remove> RemoveRuleGroup;

		/// <summary>
		/// 嫁接法则组
		/// </summary>
		public IRuleGroup<Graft> GraftRuleGroup;

		/// <summary>
		/// 裁剪法则组
		/// </summary>
		public IRuleGroup<Cut> CutRuleGroup;

		/// <summary>
		/// 激活法则组
		/// </summary>
		public IRuleGroup<Enable> EnableRuleGroup;
		/// <summary>
		/// 禁用法则组
		/// </summary>
		public IRuleGroup<Disable> DisableRuleGroup;

		/// <summary>
		/// 序列化法则组
		/// </summary>
		public IRuleGroup<Serialize> SerializeRuleGroup;

		/// <summary>
		/// 反序列化法则组
		/// </summary>
		public IRuleGroup<Deserialize> DeserializeRuleGroup;

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
		public WorldLine Create(int id, Type worldType, Type heartType, int frameTime = 0)
		{
			if (!lineDict.TryGetValue(id, out WorldLine line))
			{
				line = new WorldLine();

				if (ViewLine != null)
				{
					//可视化世界线添加可视化生成器
					INode nodeView = ViewLine.PoolGetNode(viewBuilderType);
					//ViewLine.World
					line.ViewBuilder = NodeBranchHelper.AddNodeToTree(ViewLine, default(ChildBranch), nodeView.Id, nodeView, (INode)line, default(INode)) as IWorldTreeNodeViewBuilder;
				}
				MainLine ??= line;
				line.WorldLineManager = this;
				line.Init(worldType, heartType, frameTime);
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
			NodePoolManager.Dispose();
			UnitPoolManager.Dispose();
			IdManager.Dispose();
			RealTimeManager.Dispose();
			RuleManager.Dispose();
			TypeInfo.Dispose();
		}
	}

	public static class WorldLineManagerRule
	{
		/// <summary>
		/// 新建核心对象
		/// </summary>
		public static CoreObject CreateCoreObject(this CoreObject self, Type type)
		{
			CoreObject obj = (CoreObject)Activator.CreateInstance(type);
			obj.Core = self.Core;
			obj.OnCreate();
			return obj;
		}

		/// <summary>
		/// 新建核心对象
		/// </summary>
		public static T CreateCoreObject<T>(this CoreObject self, out T newObj) where T : CoreObject, new()
		{
			newObj = new T();
			newObj.Core = self.Core;
			newObj.OnCreate();
			return newObj;
		}

		/// <summary>
		/// 新建核心对象
		/// </summary>
		public static T CreateCoreObject<T>(this CoreObject self) where T : CoreObject, new()
		{
			T newObj = new T();
			newObj.Core = self.Core;
			newObj.OnCreate();
			return newObj;
		}
	}
}