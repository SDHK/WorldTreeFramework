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
	//GF的Debug面板 ，UI管理器

	//UI 安全区 Screen.safeArea


	//不需要池的对象
	//GlobalRuleActuatorManager
	//GlobalRuleActuator<R>
	//HybridListenerRuleActuator
	//HybridListenerRuleActuatorGroup


	/// <summary>
	/// 世界线接口
	/// </summary>
	public interface IWorldLine : INode
	{
		/// <summary>
		/// 框架启动
		/// </summary>
		public void Init(Type heartType, int frameTime);
	}


	/// <summary>
	/// 世界线
	/// </summary>
	public class WorldLine : World, IWorldLine, IListenerIgnorer
		, AsCoreManagerBranch
		, AsComponentBranch
		, AsRule<Awake>
	{
		#region 字段

		/// <summary>
		/// 世界线管理器
		/// </summary>
		public WorldLineManager WorldLineManager;


		#region 日志
		/// <summary>
		/// 日志管理器
		/// </summary>
		public LogManager LogManager;

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
		/// 核心激活标记
		/// </summary>
		public bool IsCoreActive = false;

		/// <summary>
		/// Id管理器
		/// </summary>
		public IdManager IdManager;

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
		/// 数组对象池管理器
		/// </summary>
		public ArrayPoolManager ArrayPoolManager;


		/// <summary>
		/// 节点引用池管理器
		/// </summary>
		public ReferencedPoolManager ReferencedPoolManager;

		/// <summary>
		/// 全局法则执行器管理器
		/// </summary>
		public GlobalRuleExecutorManager GlobalRuleExecutorManager;

		/// <summary>
		/// 世界之心
		/// </summary>
		public WorldHeartBase WorldHeart;

		/// <summary>
		/// 世界环境：线程上下文
		/// </summary>
		public WorldContext WorldContext;

		#endregion

		#region 生命周期

		/// <summary>
		/// 框架启动
		/// </summary>
		public virtual void Init(Type heartType, int frameTime)
		{
			SetActive(false);


			//根节点初始化
			Core = this;
			World = this;

			//框架核心启动组件新建初始化

			//类型信息初始化
			TypeInfo = Activator.CreateInstance(typeof(TypeInfo), true) as TypeInfo;
			TypeInfo.Core = this;
			TypeInfo.World = this.World;
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

			this.PoolGetUnit(out LogManager);

			//法则管理器初始化
			this.PoolGetNode(out RuleManager);

			AddRuleGroup = RuleManager.GetOrNewRuleGroup<Add>();
			RemoveRuleGroup = RuleManager.GetOrNewRuleGroup<Remove>();
			BeforeRemoveRuleGroup = RuleManager.GetOrNewRuleGroup<BeforeRemove>();
			EnableRuleGroup = RuleManager.GetOrNewRuleGroup<Enable>();
			DisableRuleGroup = RuleManager.GetOrNewRuleGroup<Disable>();
			GraftRuleGroup = RuleManager.GetOrNewRuleGroup<Graft>();
			CutRuleGroup = RuleManager.GetOrNewRuleGroup<Cut>();
			SerializeRuleGroup = RuleManager.GetOrNewRuleGroup<Serialize>();
			DeserializeRuleGroup = RuleManager.GetOrNewRuleGroup<Deserialize>();

			//引用池管理器初始化
			this.PoolGetNode(out ReferencedPoolManager);

			//组件添加到树
			this.TryGraftCoreManager(ReferencedPoolManager);
			this.TryGraftCoreManager(IdManager);
			this.TryGraftCoreManager(TypeInfo);
			this.TryGraftCoreManager(RuleManager);

			//对象池组件。 out 会在执行完之前就赋值 ，但这时候对象池并没有准备好
			UnitPoolManager = this.AddCoreManager(out UnitPoolManager _);
			NodePoolManager = this.AddCoreManager(out NodePoolManager _);
			ArrayPoolManager = this.AddCoreManager(out ArrayPoolManager _);

			UnitPoolManager.TryGet(this.TypeToCode<ChildBranch>(), out _);


			//嫁接节点需要手动激活
			ReferencedPoolManager.SetActive(true);
			IdManager.SetActive(true);
			TypeInfo.SetActive(true);
			RuleManager.SetActive(true);

			//核心激活
			SetActive(true);
			IsCoreActive = true;

			//全局法则执行器管理器
			GlobalRuleExecutorManager = this.AddCoreManager(out GlobalRuleExecutorManager _);

			//真实时间管理器
			RealTimeManager = this.AddCoreManager(out RealTimeManager _);

			//游戏时间管理器
			GameTimeManager = this.AddCoreManager(out GameTimeManager _);

			long typeCode = this.TypeToCode(heartType);
			WorldHeart = NodeBranchHelper.AddNode(this, default(CoreManagerBranch), typeCode, typeCode, out _, frameTime) as WorldHeartBase;
			WorldContext = this.AddCoreManager(out WorldContext _);
			WorldHeart.Run();
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
				World = null;
				//mainCore = parent.Core.mainCore ?? this;
				SetActive(true);//激活节点
				return true;
			}
			return false;
		}

		public override void OnAddSelfToTree()
		{
			INodeProxyRule.AddNodeView(this);

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

			//有严格的移除顺序
			this.RemoveAllTemp();
			this.RemoveAllComponent();
			WorldHeart?.Dispose();
			WorldContext?.Dispose();
			World.Dispose();
			GameTimeManager?.Dispose();
			RealTimeManager?.Dispose();
			GlobalRuleExecutorManager?.Dispose();
			ArrayPoolManager?.Dispose();
			NodePoolManager?.Dispose();
			UnitPoolManager?.Dispose();
			RuleManager?.Dispose();
			IdManager?.Dispose();
			TypeInfo?.Dispose();
			ReferencedPoolManager?.Dispose();
		}

		/// <summary>
		/// 框架释放
		/// </summary>
		public override void OnDispose()
		{
			ViewBuilder?.Dispose();
			ViewBuilder = null;
			NodeBranchHelper.RemoveNode(this);//从父节点分支移除
			SetActive(false);
			Core.DisableRuleGroup?.Send(this); //禁用事件通知
			Core.RemoveRuleGroup?.Send(this);//移除事件通知
			Parent = null;//清除父节点

			this.PoolRecycle(this);//回收到池

			ReferencedPoolManager = null;
			TypeInfo = null;
			IdManager = null;
			RuleManager = null;
			UnitPoolManager = null;
			NodePoolManager = null;
			ArrayPoolManager = null;
			RealTimeManager = null;
			GameTimeManager = null;
			World = null;

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
			INodeProxyRule.AddNodeView(this);
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
			ViewBuilder?.Dispose();
			ViewBuilder = null;
			NodeRuleHelper.SendRule(this, default(Cut));
		}

		#endregion

		#endregion
	}
}