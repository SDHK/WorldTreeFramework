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


	//不需要引用的是否需要接口标记

	//Unity的节点可视化组件

	//节点分支添加需要补充Type类型，不然无法配表


	/// <summary>
	/// 世界树核心接口
	/// </summary>
	public interface IWorldTreeCore : INode
	{
		/// <summary>
		/// 框架启动
		/// </summary>
		public void Awake();
	}

	/// <summary>
	/// 世界树核心
	/// </summary>
	public class WorldTreeCore : Node, IWorldTreeCore, IListenerIgnorer
		, WorldOf<WorldTreeCore>
		, AsRule<IAwakeRule>
	{

		#region 字段

		/// <summary>
		/// 主核心
		/// </summary>
		WorldTreeCore RootCore;

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
		/// 核心激活标记
		/// </summary>
		public bool IsCoreActive = false;


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

		#endregion

		#region 生命周期

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
			if (this.Id == 0) this.Id = this.IdManager.GetId();

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
			this.TryGraftComponent(this.ReferencedPoolManager);
			this.TryGraftComponent(this.IdManager);
			this.TryGraftComponent(this.RuleManager);

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


			//核心激活标记
			this.SetActive(true);
			this.IsCoreActive = true;

			this.Root = this.AddComponent(out WorldTreeRoot _);
			this.Root.Root = this.Root;
		}
		#endregion

		#region 节点处理

		#region 添加

		public override bool TryAddSelfToTree<B, K>(K Key, INode parent)
		{
			if (parent.AddBranch<B>().TryAddNode(Key, this))
			{
				this.BranchType = TypeInfo<B>.TypeCode;
				this.Parent = parent;
				this.Core = parent.Core ?? this;
				this.Root = null;
				this.Domain = null;
				this.RootCore = parent.Core.RootCore ?? this;
				this.SetActive(true);//激活节点
				return true;
			}
			return false;
		}

		public override void OnAddSelfToTree()
		{
			this.View?.Dispose();
			this.View = this.Parent?.View != null ? Parent.View.Parent.AddChild<INode, INode>(Parent.View.Type, out _, this, Parent) as IWorldTreeNodeView : null;
			//核心独立，不入上级引用池，也不用广播
			if (this.IsActive != this.m_ActiveEventMark)//激活变更
			{
				if (this.IsActive)
				{
					this.Core.EnableRuleGroup?.Send(this);//激活事件通知
				}
				else
				{
					this.Core.DisableRuleGroup?.Send(this); //禁用事件通知
				}
			}
			this.Core.AddRuleGroup?.Send(this);//节点添加事件通知
		}
		#endregion

		#region 释放

		public override void OnBeforeDispose()
		{
			this.Core.BeforeRemoveRuleGroup?.Send(this);

			//需要提前按顺序移除
			this.RemoveAllNode<WorldBranch>();

			this.RemoveComponent<WorldTreeRoot>();
			this.RemoveComponent<GlobalRuleActuatorManager>();

			this.IsCoreActive = false;

			this.RemoveComponent<GameTimeManager>();
			this.RemoveComponent<ArrayPoolManager>();
			this.RemoveComponent<NodePoolManager>();
			this.RemoveComponent<UnitPoolManager>();
			this.RemoveComponent<RuleManager>();
			this.RemoveComponent<RealTimeManager>();
			this.RemoveComponent<IdManager>();
			this.RemoveComponent<ReferencedPoolManager>();

			this.RemoveAllNode();
		}

		/// <summary>
		/// 框架释放
		/// </summary>
		public override void OnDispose()
		{
			this.View?.Dispose();
			this.View = null;
			this.Parent?.RemoveBranchNode(this.BranchType, this);//从父节点分支移除
			this.SetActive(false);
			this.Core.DisableRuleGroup?.Send(this); //禁用事件通知
			this.Core.RemoveRuleGroup?.Send(this);//移除事件通知
			this.Parent = null;//清除父节点

			this.PoolRecycle(this);//回收到池

			this.ReferencedPoolManager = null;
			this.IdManager = null;
			this.RealTimeManager = null;
			this.RuleManager = null;
			this.UnitPoolManager = null;
			this.NodePoolManager = null;
			this.ArrayPoolManager = null;
			this.Root = null;

			this.NewRuleGroup = null;
			this.GetRuleGroup = null;
			this.RecycleRuleGroup = null;
			this.DestroyRuleGroup = null;

			this.AddRuleGroup = null;
			this.RemoveRuleGroup = null;
			this.EnableRuleGroup = null;
			this.DisableRuleGroup = null;
		}

		#endregion

		#region 嫁接

		public override bool TryGraftSelfToTree<B, K>(K key, INode parent)
		{
			if (!parent.AddBranch<B>().TryAddNode(key, this)) return false;

			this.BranchType = TypeInfo<B>.TypeCode;
			this.Parent = parent;
			this.RefreshActive();
			this.TraversalLevel(current => current.OnGraftSelfToTree());
			return true;
		}

		public override void OnGraftSelfToTree()
		{
			this.View?.Dispose();
			this.View = this.Parent?.View != null ? Parent.View.Parent.AddChild<INode, INode>(Parent.View.Type, out _, this, Parent) as IWorldTreeNodeView : null;
			if (this.IsActive != this.m_ActiveEventMark)//激活变更
			{
				if (this.IsActive)
				{
					this.Core.EnableRuleGroup?.Send(this);//激活事件通知
				}
				else
				{
					this.Core.DisableRuleGroup?.Send(this); //禁用事件通知
				}
			}
			this.SendRule(TypeInfo<IGraftRule>.Default);//节点嫁接事件通知
		}

		#endregion

		#region 裁剪

		public override void OnCutSelf()
		{
			this.View?.Dispose();
			this.View = null;
			this.SendRule(TypeInfo<ICutRule>.Default);
		}

		#endregion

		#endregion
	}

	public static partial class WorldTreeCoreRule
	{
		class AddRule : AddRule<WorldTreeCore>
		{
			protected override void OnEvent(WorldTreeCore self)
			{
				self.Log = self.Core.Log;
				self.LogWarning = self.Core.LogWarning;
				self.LogError = self.Core.LogError;
				self.Awake();
				self.Log($"核心启动{self.Id}");
			}
		}
	}
}