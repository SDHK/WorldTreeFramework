/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/18 9:35

* 描述： 世界树核心
* 
* 框架的启动入口，根节点
* 
* 管理分发全局的节点与组件的生命周期
* 

*/

using System;
using System.Diagnostics;

namespace WorldTree
{
	//计划 7AGLUH5U

	//新增TimeUpdate,特化的双方法法则

	//真实与游戏时间双法则

	//真实时间和游戏时间

	//时域与时间轮



	//对象池需要一个启动标记？

	//对法则执行器进行更加详细的划分，生命周期，全局事件，回调事件？

	//管理器改为分支 整理

	//Log改为 Self.Log

	//核心分布到法则


	/// <summary>
	/// 世界树核心
	/// </summary>
	public class WorldTreeCore : CoreNode
	{
		/// <summary>
		/// 打印日志
		/// </summary>
		public Action<object> Log;
		/// <summary>
		/// 打印警告日志
		/// </summary>
		public Action<object> LogWarning;
		/// <summary>
		/// 打印错误日志
		/// </summary>
		public Action<object> LogError;

		public IRuleGroup<IAddRule> AddRuleGroup;
		public IRuleGroup<IBeforeRemoveRule> BeforeRemoveRuleGroup;
		public IRuleGroup<IRemoveRule> RemoveRuleGroup;
		public IRuleGroup<IEnableRule> EnableRuleGroup;
		public IRuleGroup<IDisableRule> DisableRuleGroup;

		public IRuleGroup<INewRule> NewRuleGroup;
		public IRuleGroup<IGetRule> GetRuleGroup;
		public IRuleGroup<IRecycleRule> RecycleRuleGroup;
		public IRuleGroup<IDestroyRule> DestroyRuleGroup;



		/// <summary>
		/// Id管理器
		/// </summary>
		public IdManager IdManager;

		/// <summary>
		/// 真实时间管理器
		/// </summary>
		public RealTimeManager RealTimeManager;

		/// <summary>
		/// 游戏时间管理器
		/// </summary>
		public GameTimeManager GameTimeManager;

		/// <summary>
		/// 法则管理器
		/// </summary>
		public RuleManager RuleManager;
		/// <summary>
		/// 单位对象池管理器
		/// </summary>
		public UnitPoolManager UnitPoolManager;
		/// <summary>
		/// 节点对象池管理器
		/// </summary>
		public NodePoolManager NodePoolManager;
		/// <summary>
		/// 节点引用池管理器
		/// </summary>
		public ReferencedPoolManager ReferencedPoolManager;
		/// <summary>
		/// 数组对象池管理器
		/// </summary>
		public ArrayPoolManager ArrayPoolManager;


		public GlobalRuleActuator<IEnableRule> enable;
		public GlobalRuleActuator<IDisableRule> disable;
		public GlobalRuleActuator<IUpdateRule> update;
		public GlobalRuleActuator<IUpdateTimeRule> updateTime;


		/// <summary>
		/// 释放
		/// </summary>
		public override void Dispose()
		{
			this.Destroy();
		}
	}


	public static partial class WorldTreeCoreRule
	{
		#region 日志

		/// <summary>
		/// 打印日志
		/// </summary>
		public static void Log(this IUnitPoolItem self, object message) => self.Core.Log?.Invoke(message);
		/// <summary>
		/// 打印警告日志
		/// </summary>
		public static void LogWarning(this IUnitPoolItem self, object message) => self.Core.LogWarning?.Invoke(message);
		/// <summary>
		/// 打印错误日志
		/// </summary>
		public static void LogError(this IUnitPoolItem self, object message) => self.Core.LogError?.Invoke(message);

		#endregion

		#region 框架启动与销毁

		/// <summary>
		/// 框架启动
		/// </summary>
		public static void Awake(this WorldTreeCore self)
		{
			//根节点初始化
			self.Type = TypeInfo<WorldTreeCore>.TypeCode;
			self.Core = self;
			self.Domain = self;

			//框架核心启动组件新建初始化

			//Id管理器初始化
			self.NewNode(out self.IdManager);
			self.Id = self.IdManager.GetId();

			//时间管理器初始化
			self.NewNode(out self.RealTimeManager);

			//法则管理器初始化
			self.NewNode(out self.RuleManager);

			self.NewRuleGroup = self.RuleManager.GetOrNewRuleGroup<INewRule>();
			self.GetRuleGroup = self.RuleManager.GetOrNewRuleGroup<IGetRule>();
			self.BeforeRemoveRuleGroup = self.RuleManager.GetOrNewRuleGroup<IBeforeRemoveRule>();
			self.RecycleRuleGroup = self.RuleManager.GetOrNewRuleGroup<IRecycleRule>();
			self.DestroyRuleGroup = self.RuleManager.GetOrNewRuleGroup<IDestroyRule>();

			self.AddRuleGroup = self.RuleManager.GetOrNewRuleGroup<IAddRule>();
			self.RemoveRuleGroup = self.RuleManager.GetOrNewRuleGroup<IRemoveRule>();
			self.EnableRuleGroup = self.RuleManager.GetOrNewRuleGroup<IEnableRule>();
			self.DisableRuleGroup = self.RuleManager.GetOrNewRuleGroup<IDisableRule>();

			//引用池管理器初始化
			self.NewNodeLifecycle(out self.ReferencedPoolManager);

			//组件添加到树
			self.GraftComponent(self.ReferencedPoolManager);
			self.GraftComponent(self.IdManager);
			self.GraftComponent(self.RuleManager);

			//对象池组件。 out 会在执行完之前就赋值 ，但这时候对象池并没有准备好
			self.UnitPoolManager = self.AddComponent(out UnitPoolManager _, isPool: false);
			self.NodePoolManager = self.AddComponent(out NodePoolManager _, isPool: false);
			self.ArrayPoolManager = self.AddComponent(out ArrayPoolManager _, isPool: false);

			self.UnitPoolManager.TryGet(TypeInfo<ChildBranch>.TypeCode, out _);

			//树根节点
			self.GraftComponent(self.Root = self.PoolGetNode<WorldTreeRoot>());

			//嫁接节点需要手动激活
			self.ReferencedPoolManager.SetActive(true);
			self.IdManager.SetActive(true);
			self.RuleManager.SetActive(true);
			self.Root.SetActive(true);

			//游戏时间管理器
			//self.AddComponent(out self.GameTimeManager);

			//核心激活
			self.SetActive(true);

			self.GetOrNewGlobalRuleActuator(out self.enable);
			self.GetOrNewGlobalRuleActuator(out self.update);
			self.GetOrNewGlobalRuleActuator(out self.updateTime);
			self.GetOrNewGlobalRuleActuator(out self.disable);
		}

		/// <summary>
		/// 框架销毁
		/// </summary>
		public static void Destroy(this WorldTreeCore self)
		{

			self.SetActive(false);
			self.RemoveComponent<WorldTreeRoot>();
			self.RemoveComponent<GameTimeManager>();
			self.RemoveComponent<ArrayPoolManager>();
			self.RemoveComponent<NodePoolManager>();
			self.RemoveComponent<UnitPoolManager>();
			self.RemoveComponent<RuleManager>();
			self.RemoveComponent<RealTimeManager>();
			self.RemoveComponent<IdManager>();
			self.RemoveComponent<ReferencedPoolManager>();

			self.RemoveAllNode();

			self.NewRuleGroup = null;
			self.GetRuleGroup = null;
			self.RecycleRuleGroup = null;
			self.DestroyRuleGroup = null;


			self.AddRuleGroup = null;
			self.RemoveRuleGroup = null;
			self.EnableRuleGroup = null;
			self.DisableRuleGroup = null;

			self.ReferencedPoolManager = null;
			self.IdManager = null;
			self.RealTimeManager = null;
			self.RuleManager = null;
			self.UnitPoolManager = null;
			self.NodePoolManager = null;
			self.ArrayPoolManager = null;
			self.Root = null;

			self.enable = null;
			self.update = null;
			self.updateTime = null;
			self.disable = null;
		}
		#endregion

		/// <summary>
		/// 框架刷新
		/// </summary>
		public static void Update(this WorldTreeCore self, float deltaTime)
		{
			self.enable?.Send();
			self.update?.Send();
			self.updateTime?.Send(deltaTime);
			self.disable?.Send();
		}
	}
}
