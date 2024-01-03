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

namespace WorldTree
{
	//计划

	//藤蔓网状结构

	//新增TimeUpdate,特化的双方法法则

	//真实与游戏时间双法则

	//真实时间和游戏时间

	//时域与时间轮


	//管理器改为分支 整理

	//世界线程，界程

	//切线程，同步上下文

	//邮箱组件需要管理器Update
	//节点邮箱组件，只有SendMail<1,2,3,4,5>

	//核心处理重写

	//多线程核心设计





	/// <summary>
	/// 世界树核心接口
	/// </summary>
	public interface IWorldTreeCore : INode
	{
		/// <summary>
		/// 框架启动
		/// </summary>
		public void Awake();

		/// <summary>
		/// 框架刷新
		/// </summary>
		public void Update(float deltaTime);

		///// <summary>
		///// 框架异步释放
		///// </summary>
		//public TreeTask DisposeAsync();
	}


	/// <summary>
	/// 世界树核心
	/// </summary>
	public class WorldTreeCore : Node, IWorldTreeCore, IListenerIgnorer
		, WorldOf<WorldTreeCore>
		, AsRule<IAwakeRule>
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

		/// <summary>
		/// 世界环境
		/// </summary>
		public WorldContext worldContext;


		public GlobalRuleActuator<IEnableRule> enable;
		public GlobalRuleActuator<IDisableRule> disable;
		public GlobalRuleActuator<IUpdateRule> update;
		public GlobalRuleActuator<IUpdateTimeRule> updateTime;


		public virtual void Awake()
		{
			this.SetActive(false);

			//根节点初始化
			this.Type = TypeInfo<WorldTreeCore>.TypeCode;
			this.Core = this;
			this.Domain = this;
			this.Root = null;

			//框架核心启动组件新建初始化

			//Id管理器初始化
			this.NewNode(out this.IdManager);
			if (this.Id == 0)
			{
				this.Id = this.IdManager.GetId();
			}

			//时间管理器初始化
			this.NewNode(out this.RealTimeManager);

			//法则管理器初始化
			this.NewNode(out this.RuleManager);

			this.NewRuleGroup = this.RuleManager.GetOrNewRuleGroup<INewRule>();
			this.GetRuleGroup = this.RuleManager.GetOrNewRuleGroup<IGetRule>();
			this.BeforeRemoveRuleGroup = this.RuleManager.GetOrNewRuleGroup<IBeforeRemoveRule>();
			this.RecycleRuleGroup = this.RuleManager.GetOrNewRuleGroup<IRecycleRule>();
			this.DestroyRuleGroup = this.RuleManager.GetOrNewRuleGroup<IDestroyRule>();

			this.AddRuleGroup = this.RuleManager.GetOrNewRuleGroup<IAddRule>();
			this.RemoveRuleGroup = this.RuleManager.GetOrNewRuleGroup<IRemoveRule>();
			this.EnableRuleGroup = this.RuleManager.GetOrNewRuleGroup<IEnableRule>();
			this.DisableRuleGroup = this.RuleManager.GetOrNewRuleGroup<IDisableRule>();

			//引用池管理器初始化
			this.NewNodeLifecycle(out this.ReferencedPoolManager);

			//组件添加到树
			this.GraftComponent(this.ReferencedPoolManager);
			this.GraftComponent(this.IdManager);
			this.GraftComponent(this.RuleManager);

			//对象池组件。 out 会在执行完之前就赋值 ，但这时候对象池并没有准备好
			this.UnitPoolManager = this.AddComponent(out UnitPoolManager _, isPool: false);
			this.NodePoolManager = this.AddComponent(out NodePoolManager _, isPool: false);
			this.ArrayPoolManager = this.AddComponent(out ArrayPoolManager _, isPool: false);

			this.UnitPoolManager.TryGet(TypeInfo<ChildBranch>.TypeCode, out _);

			//嫁接节点需要手动激活
			this.ReferencedPoolManager.SetActive(true);
			this.IdManager.SetActive(true);
			this.RuleManager.SetActive(true);
			//self.Root.SetActive(true);

			//游戏时间管理器
			//self.AddComponent(out self.GameTimeManager);


			//核心激活
			this.SetActive(true);


			this.GetOrNewGlobalRuleActuator(out this.enable);
			this.GetOrNewGlobalRuleActuator(out this.update);
			this.GetOrNewGlobalRuleActuator(out this.updateTime);
			this.GetOrNewGlobalRuleActuator(out this.disable);

			this.Root = this.AddComponent(out WorldTreeRoot _);
			this.Root.Root = this.Root;
		}
		public virtual void Update(float deltaTime)
		{
			this.enable?.Send();
			this.update?.Send();
			this.updateTime?.Send(deltaTime);
			this.disable?.Send();
		}


		//整理：添加，嫁接，裁剪 


		public override void OnBeforeDispose()
		{
			this.Core.BeforeRemoveRuleGroup?.Send(this);

			//需要提前按顺序移除
			this.RemoveAllNode<WorldBranch>();

			this.RemoveComponent<WorldTreeRoot>();
			this.RemoveComponent<GlobalRuleActuatorManager>();

			this.Parent?.RemoveBranchNode(this.BranchType, this);//从父节点分支移除
			this.SetActive(false);
			this.RemoveComponent<GameTimeManager>();
			this.RemoveComponent<ArrayPoolManager>();
			this.RemoveComponent<NodePoolManager>();
			this.RemoveComponent<UnitPoolManager>();
			this.RemoveComponent<RuleManager>();
			this.RemoveComponent<RealTimeManager>();
			this.RemoveComponent<IdManager>();
			this.RemoveComponent<ReferencedPoolManager>();

			this.RemoveAllNode();

			this.NewRuleGroup = null;
			this.GetRuleGroup = null;
			this.RecycleRuleGroup = null;
			this.DestroyRuleGroup = null;


			this.AddRuleGroup = null;
			this.RemoveRuleGroup = null;
			this.EnableRuleGroup = null;
			this.DisableRuleGroup = null;

			this.ReferencedPoolManager = null;
			this.IdManager = null;
			this.RealTimeManager = null;
			this.RuleManager = null;
			this.UnitPoolManager = null;
			this.NodePoolManager = null;
			this.ArrayPoolManager = null;
			this.Root = null;

			this.enable = null;
			this.update = null;
			this.updateTime = null;
			this.disable = null;
		}

		/// <summary>
		/// 框架释放
		/// </summary>
		public override void OnDispose() { }
	}


	public static partial class WorldTreeCoreRule
	{

		class AddRule : AddRule<WorldTreeCore>
		{
			protected override void OnEvent(WorldTreeCore self)
			{
			}
		}

		class RemoveRule : RemoveRule<WorldTreeCore>
		{
			protected override void OnEvent(WorldTreeCore self)
			{
			}
		}
	}
}
