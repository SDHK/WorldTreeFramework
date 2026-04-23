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
		public void Init(Type worldType, Type heartType, int frameTime);
	}

	/// <summary>
	/// 世界线
	/// </summary>
	public class WorldLine : Node, IWorldLine, IListenerIgnorer
		, AsCoreManagerBranch
		, AsComponentBranch
		, AsRule<Awake>
	{
		#region 字段

		/// <summary>
		/// 世界树核心
		/// </summary>
		public WorldTreeCore WorldLineManager;

		/// <summary>
		/// 日志管理器
		/// </summary>
		public LogManager LogManager;

		/// <summary>
		/// 世界之心
		/// </summary>
		public WorldHeartBase WorldHeart;

		/// <summary>
		/// 世界环境：线程上下文
		/// </summary>
		public WorldContext WorldContext;

		/// <summary>
		/// 世界时间管理器
		/// </summary>
		public GameTimeManager GameTimeManager;

		//真实时间定时器

		/// <summary>
		/// 节点引用池管理器
		/// </summary>
		public ReferencedPoolManager ReferencedPoolManager;

		/// <summary>
		/// 全局法则执行器管理器
		/// </summary>
		public RuleBroadcastManager GlobalRuleExecutorManager;

		/// <summary>
		/// 主世界 
		/// </summary>
		public World MainWorld;

		#endregion

		#region 生命周期

		/// <summary>
		/// 框架启动
		/// </summary>
		public virtual void Init(Type worldType, Type heartType, int frameTime)
		{
			SetActive(false);

			//根节点初始化
			Core = this;

			//框架核心启动组件新建初始化
			Type = this.TypeToCode();


			//日志管理器初始化
			this.WorldLineManager.CreateCoreObject(out LogManager);
			LogManager.Init(Id.ToString());

			//引用池管理器初始化
			this.PoolGetNode(out ReferencedPoolManager);

			//组件添加到树
			this.TryGraftCoreManager(ReferencedPoolManager);

			//嫁接节点需要手动激活
			ReferencedPoolManager.SetActive(true);

			//核心激活
			SetActive(true);

			//全局法则执行器管理器
			GlobalRuleExecutorManager = this.AddCoreManager(out RuleBroadcastManager _);

			//游戏时间管理器
			GameTimeManager = this.AddCoreManager(out GameTimeManager _);

			long typeCode = this.TypeToCode(heartType);
			WorldHeart = NodeBranchHelper.AddNode(this, default(CoreManagerBranch), typeCode, heartType, out _, frameTime) as WorldHeartBase;
			WorldContext = this.AddCoreManager(out WorldContext _);
			WorldHeart.Run();

			WorldContext.Post(() =>
			{
				NodeBranchHelper.AddNode(this, default(ComponentBranch), this.TypeToCode(worldType), worldType, out _);
			});
		}

		#endregion

		#region 节点处理

		#region 添加

		public override bool TryAddSelfToTree<B, K>(K key, INode parent)
		{
			if (NodeBranchHelper.AddBranch<B>(parent).TryAddNode(key, this))
			{
				BranchType = TypeInfo<B>.Code;
				Core = parent.Core ?? this;
				World = null;
				Parent = parent;
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
					WorldLineManager.EnableRuleGroup?.Send(this);//激活事件通知
				}
				else
				{
					WorldLineManager.DisableRuleGroup?.Send(this); //禁用事件通知
				}
			}
			WorldLineManager.AddRuleGroup?.Send(this);//节点添加事件通知
		}

		#endregion

		#region 释放

		/// <summary>
		/// 回收节点
		/// </summary>
		public override void Dispose()
		{
			//是否已经回收
			if (IsDisposed) return;

			//节点回收前序遍历处理,节点回收后续遍历处理
			NodeBranchTraversalHelper.TraversalPrePostOrder(this, current => current.OnBeforeDispose(), current => current.OnDispose());
		}

		public override void OnBeforeDispose()
		{
			WorldLineManager.BeforeRemoveRuleGroup?.Send(this);

			//有严格的移除顺序
			this.RemoveAllTemp();
			this.RemoveAllComponent();
			WorldHeart?.Dispose();
			WorldContext?.Dispose();
			World.Dispose();
			GameTimeManager?.Dispose();
			GlobalRuleExecutorManager?.Dispose();
			ReferencedPoolManager?.Dispose();
			LogManager?.Dispose();
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
			WorldLineManager.DisableRuleGroup?.Send(this); //禁用事件通知
			WorldLineManager.RemoveRuleGroup?.Send(this);//移除事件通知
			Parent = null;//清除父节点

			this.PoolRecycle(this);//回收到池

			ReferencedPoolManager = null;
			GameTimeManager = null;
			World = null;
		}

		#endregion

		#endregion
	}
}