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
	//TODO 计划
	//UI 安全区 Screen.safeArea

	//藤蔓网状结构

	//管理器改为分支 整理

	//世界线程，界程

	//切线程，同步上下文

	//邮箱组件需要管理器Update
	//节点邮箱组件，只有SendMail<1,2,3,4,5>

	//域节点接口标记

	//代码表格

	//YooAsset管理器


	//真实与游戏时间双法则

	//时域与时间轮

	//TaskBox改名

	//代码生成Destroy，让引用=null 


	//不需要池的对象
	//GlobalRuleActuatorManager
	//GlobalRuleActuator<R>
	//HybridListenerRuleActuator
	//HybridListenerRuleActuatorGroup


	// 序列化 ：	
	// 类型码用正数下标 ：127个内为：1byte，
	// 引用码用负数下标 ：-120个内为：1byte
	// 负数0~120 标记引用列表， 后续跟标记类型码



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
		/// 序列化法则组
		/// </summary>
		public IRuleGroup<Serialize> SerializeRuleGroup;

		/// <summary>
		/// 反序列化法则组
		/// </summary>
		public IRuleGroup<Deserialize> DeserializeRuleGroup;

		/// <summary>
		/// 核心激活标记
		/// </summary>
		public bool IsCoreActive = false;

		/// <summary>
		/// Id管理器
		/// </summary>
		public IdManager IdManager;

		/// <summary>
		/// 代码加载器
		/// </summary>
		public CodeLoader CodeLoader;

		/// <summary>
		/// 类型信息
		/// </summary>
		public TypeInfo TypeInfo;



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
			SetActive(false);

			//根节点初始化
			Core = this;
			Domain = this;
			Root = null;

			//框架核心启动组件新建初始化

			//类型信息初始化
			TypeInfo = Activator.CreateInstance(typeof(TypeInfo), true) as TypeInfo;
			TypeInfo.Core = this;
			TypeInfo.Root = this.Root;
			TypeInfo.Type = TypeInfo.TypeToCode(typeof(TypeInfo));
			TypeInfo.OnCreate();


			Type = this.TypeToCode();

			//Id管理器初始化
			this.PoolGetNode(out IdManager);
			if (Id == 0)
			{
				InstanceId = IdManager.GetId();
				Id = InstanceId;
			}

			TypeInfo.InstanceId = IdManager.GetId();
			TypeInfo.Id = TypeInfo.InstanceId;

			//法则管理器初始化
			this.PoolGetNode(out RuleManager);

			BeforeRemoveRuleGroup = RuleManager.GetOrNewRuleGroup<BeforeRemove>();

			AddRuleGroup = RuleManager.GetOrNewRuleGroup<Add>();
			RemoveRuleGroup = RuleManager.GetOrNewRuleGroup<Remove>();
			EnableRuleGroup = RuleManager.GetOrNewRuleGroup<Enable>();
			DisableRuleGroup = RuleManager.GetOrNewRuleGroup<Disable>();
			GraftRuleGroup = RuleManager.GetOrNewRuleGroup<Graft>();
			CutRuleGroup = RuleManager.GetOrNewRuleGroup<Cut>();
			SerializeRuleGroup = RuleManager.GetOrNewRuleGroup<Serialize>();
			DeserializeRuleGroup = RuleManager.GetOrNewRuleGroup<Deserialize>();

			//引用池管理器初始化
			this.PoolGetNode(out ReferencedPoolManager);

			//组件添加到树
			this.TryGraftComponent(ReferencedPoolManager);
			this.TryGraftComponent(IdManager);
			this.TryGraftComponent(TypeInfo);
			this.TryGraftComponent(RuleManager);

			//对象池组件。 out 会在执行完之前就赋值 ，但这时候对象池并没有准备好
			UnitPoolManager = this.AddComponent(out UnitPoolManager _);
			NodePoolManager = this.AddComponent(out NodePoolManager _);
			ArrayPoolManager = this.AddComponent(out ArrayPoolManager _);

			UnitPoolManager.TryGet(this.TypeToCode<ChildBranch>(), out _);

			//嫁接节点需要手动激活
			ReferencedPoolManager.SetActive(true);
			IdManager.SetActive(true);
			TypeInfo.SetActive(true);
			RuleManager.SetActive(true);

			//核心激活
			SetActive(true);
			IsCoreActive = true;

			//真实时间管理器
			RealTimeManager = this.AddComponent(out RealTimeManager _);

			//游戏时间管理器
			GameTimeManager = this.AddComponent(out GameTimeManager _);

			Root = this.AddComponent(out WorldTreeRoot _);
			Root.Root = Root;
			WorldContext = Root.AddComponent(out WorldContext _);
		}

		#endregion

		#region 节点处理

		#region 添加

		public override bool TryAddSelfToTree<B, K>(K key, INode parent)
		{
			if (NodeBranchHelper.AddBranch<B>(parent).TryAddNode(key, this))
			{
				BranchType = Core.TypeToCode<B>();
				Parent = parent;
				Core = parent.Core ?? this;
				Root = null;
				Domain = null;
				rootCore = parent.Core.rootCore ?? this;
				SetActive(true);//激活节点
				return true;
			}
			return false;
		}

		public override void OnAddSelfToTree()
		{
			AddNodeView();

			//核心独立，不入上级引用池，也不用广播
			if (IsActive != activeEventMark)//激活变更
			{
				if (IsActive)
				{
					Core.EnableRuleGroup?.Send(this);//激活事件通知
				}
				else
				{
					Core.DisableRuleGroup?.Send(this); //禁用事件通知
				}
			}
			Core.AddRuleGroup?.Send(this);//节点添加事件通知
		}

		#endregion

		#region 释放

		/// <summary>
		/// 回收节点
		/// </summary>
		public override void Dispose()
		{
			//核心激活关闭
			IsCoreActive = false;

			//是否已经回收
			if (IsDisposed) return;

			//节点回收前序遍历处理,节点回收后续遍历处理
			NodeBranchTraversalHelper.TraversalPrePostOrder(this, current => current.OnBeforeDispose(), current => current.OnDispose());
		}

		public override void OnBeforeDispose()
		{
			Core.BeforeRemoveRuleGroup?.Send(this);

			//需要提前按顺序移除
			this.RemoveAllWorld();
			this.RemoveComponent<WorldTreeCore, WorldTreeRoot>();
			this.RemoveComponent<WorldTreeCore, GameTimeManager>();
			this.RemoveComponent<WorldTreeCore, RealTimeManager>();
			this.RemoveComponent<WorldTreeCore, GlobalRuleExecutorManager>();
			this.RemoveComponent<WorldTreeCore, ArrayPoolManager>();
			this.RemoveComponent<WorldTreeCore, NodePoolManager>();
			this.RemoveComponent<WorldTreeCore, UnitPoolManager>();
			this.RemoveComponent<WorldTreeCore, RuleManager>();
			this.RemoveComponent<WorldTreeCore, IdManager>();
			this.RemoveComponent<WorldTreeCore, TypeInfo>();
			this.RemoveComponent<WorldTreeCore, ReferencedPoolManager>();


			RemoveAllNode();
		}

		/// <summary>
		/// 框架释放
		/// </summary>
		public override void OnDispose()
		{
			View?.Dispose();
			View = null;
			NodeBranchHelper.RemoveNode(this);//从父节点分支移除
			SetActive(false);
			Core.DisableRuleGroup?.Send(this); //禁用事件通知
			Core.RemoveRuleGroup?.Send(this);//移除事件通知
			Parent = null;//清除父节点

			this.PoolRecycle(this);//回收到池

			ReferencedPoolManager = null;
			IdManager = null;
			RealTimeManager = null;
			RuleManager = null;
			UnitPoolManager = null;
			NodePoolManager = null;
			ArrayPoolManager = null;
			Root = null;

			AddRuleGroup = null;
			RemoveRuleGroup = null;
			EnableRuleGroup = null;
			DisableRuleGroup = null;
		}

		#endregion

		#region 嫁接

		public override bool TryGraftSelfToTree<B, K>(K key, INode parent)
		{
			if (!NodeBranchHelper.AddBranch<B>(parent).TryAddNode(key, this)) return false;

			BranchType = Core.TypeToCode<B>();
			Parent = parent;
			RefreshActive();
			NodeBranchTraversalHelper.TraversalLevel(this, current => current.OnGraftSelfToTree());
			return true;
		}
		public override void OnBeforeGraftSelfToTree()
		{
			AddNodeView();
		}
		public override void OnGraftSelfToTree()
		{
			if (IsActive != activeEventMark)//激活变更
			{
				if (IsActive)
				{
					Core.EnableRuleGroup?.Send(this);//激活事件通知
				}
				else
				{
					Core.DisableRuleGroup?.Send(this); //禁用事件通知
				}
			}
			NodeRuleHelper.SendRule(this, default(Graft));//节点嫁接事件通知
		}

		#endregion

		#region 裁剪

		public override void OnCutSelf()
		{
			View?.Dispose();
			View = null;
			NodeRuleHelper.SendRule(this, default(Cut));
		}

		#endregion

		#endregion
	}

	public static partial class WorldTreeCoreRule
	{
		private class AddRule1 : AddRule<WorldTreeCore>
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