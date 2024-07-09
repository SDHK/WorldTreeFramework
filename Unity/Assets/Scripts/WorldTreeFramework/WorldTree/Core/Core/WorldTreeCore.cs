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
using System.Reflection;
using WorldTree.Internal;

namespace WorldTree
{
	//计划

	//藤蔓网状结构

	//管理器改为分支 整理

	//世界线程，界程

	//切线程，同步上下文

	//邮箱组件需要管理器Update
	//节点邮箱组件，只有SendMail<1,2,3,4,5>

	//域节点接口标记

	//代码表格

	//YooAsset管理器

	//需要定时器TimeUpdate,特化的双方法法则？

	//真实与游戏时间双法则

	//时域与时间轮

	//TaskBox改名

	//代码生成Destroy，让引用=null 

	//id赋值时机，UID和普通id 的区分，序列化和普通类型的id

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
		, AsComponentBranch
		, AsWorldBranch
		, WorldOf<WorldTreeCore>
		, AsAwake
	{
		#region 字段

		/// <summary>
		/// 程序集
		/// </summary>
		//public Assembly[] Assemblys;

		/// <summary>
		/// 主核心
		/// </summary>
		private WorldTreeCore rootCore;

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
		public WorldContext WorldContext;

		#endregion

		#region 生命周期

		/// <summary>
		/// 框架启动
		/// </summary>
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

			//法则管理器初始化
			this.NewNode(out this.RuleManager);

			this.BeforeRemoveRuleGroup = this.RuleManager.GetOrNewRuleGroup<BeforeRemove>();

			this.AddRuleGroup = this.RuleManager.GetOrNewRuleGroup<Add>();
			this.RemoveRuleGroup = this.RuleManager.GetOrNewRuleGroup<Remove>();
			this.EnableRuleGroup = this.RuleManager.GetOrNewRuleGroup<Enable>();
			this.DisableRuleGroup = this.RuleManager.GetOrNewRuleGroup<Disable>();
			this.GraftRuleGroup = this.RuleManager.GetOrNewRuleGroup<Graft>();
			this.CutRuleGroup = this.RuleManager.GetOrNewRuleGroup<Cut>();

			//引用池管理器初始化
			this.NewNode(out this.ReferencedPoolManager);

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

			//核心激活
			this.SetActive(true);
			this.IsCoreActive = true;

			//真实时间管理器
			this.RealTimeManager = this.AddComponent(out RealTimeManager _);

			//游戏时间管理器
			this.GameTimeManager = this.AddComponent(out GameTimeManager _);

			this.Root = this.AddComponent(out WorldTreeRoot _);
			this.Root.Root = this.Root;
			this.WorldContext = this.Root.AddComponent(out WorldContext _);
		}

		#endregion

		#region 节点处理

		#region 添加

		public override bool TryAddSelfToTree<B, K>(K key, INode parent)
		{
			if (NodeBranchHelper.AddBranch<B>(parent).TryAddNode(key, this))
			{
				this.branchType = TypeInfo<B>.TypeCode;
				this.Parent = parent;
				this.Core = parent.Core ?? this;
				this.Root = null;
				this.Domain = null;
				this.rootCore = parent.Core.rootCore ?? this;
				this.SetActive(true);//激活节点
				return true;
			}
			return false;
		}

		public override void OnAddSelfToTree()
		{
			AddNodeView();

			//核心独立，不入上级引用池，也不用广播
			if (this.IsActive != this.activeEventMark)//激活变更
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

		/// <summary>
		/// 回收节点
		/// </summary>
		public override void Dispose()
		{
			//核心激活关闭
			this.IsCoreActive = false;

			//是否已经回收
			if (this.IsDisposed) return;

			//节点回收前序遍历处理,节点回收后续遍历处理
			NodeBranchTraversalHelper.TraversalPrePostOrder(this, current => current.OnBeforeDispose(), current => current.OnDispose());
		}

		public override void OnBeforeDispose()
		{
			this.Core.BeforeRemoveRuleGroup?.Send(this);

			//需要提前按顺序移除
			this.RemoveAllWorld();
			this.RemoveComponent<WorldTreeCore, WorldTreeRoot>();
			this.RemoveComponent<WorldTreeCore, GameTimeManager>();
			this.RemoveComponent<WorldTreeCore, RealTimeManager>();
			this.RemoveComponent<WorldTreeCore, GlobalRuleActuatorManager>();
			this.RemoveComponent<WorldTreeCore, ArrayPoolManager>();
			this.RemoveComponent<WorldTreeCore, NodePoolManager>();
			this.RemoveComponent<WorldTreeCore, UnitPoolManager>();
			this.RemoveComponent<WorldTreeCore, RuleManager>();
			this.RemoveComponent<WorldTreeCore, IdManager>();
			this.RemoveComponent<WorldTreeCore, ReferencedPoolManager>();

			this.RemoveAllNode();
		}

		/// <summary>
		/// 框架释放
		/// </summary>
		public override void OnDispose()
		{
			this.View?.Dispose();
			this.View = null;
			NodeBranchHelper.RemoveBranchNode(this.Parent, this.BranchType, this);//从父节点分支移除
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

			this.AddRuleGroup = null;
			this.RemoveRuleGroup = null;
			this.EnableRuleGroup = null;
			this.DisableRuleGroup = null;
		}

		#endregion

		#region 嫁接

		public override bool TryGraftSelfToTree<B, K>(K key, INode parent)
		{
			if (!NodeBranchHelper.AddBranch<B>(parent).TryAddNode(key, this)) return false;

			this.branchType = TypeInfo<B>.TypeCode;
			this.Parent = parent;
			this.RefreshActive();
			NodeBranchTraversalHelper.TraversalLevel(this, current => current.OnGraftSelfToTree());
			return true;
		}

		public override void OnGraftSelfToTree()
		{
			AddNodeView();
			if (this.IsActive != this.activeEventMark)//激活变更
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
			NodeRuleHelper.SendRule(this, TypeInfo<Graft>.Default);//节点嫁接事件通知
		}

		#endregion

		#region 裁剪

		public override void OnCutSelf()
		{
			this.View?.Dispose();
			this.View = null;
			NodeRuleHelper.SendRule(this, TypeInfo<Cut>.Default);
		}

		#endregion

		#endregion
	}

	public static partial class WorldTreeCoreRule
	{
		private class AddRule : AddRule<WorldTreeCore>
		{
			protected override void Execute(WorldTreeCore self)
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