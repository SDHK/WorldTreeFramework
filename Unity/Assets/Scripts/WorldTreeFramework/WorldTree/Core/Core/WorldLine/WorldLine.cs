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
	public class WorldLine : World, IWorldLine, IListenerIgnorer
		, AsComponentBranch
		, AsRule<Awake>
	{
		#region 字段

		/// <summary>
		/// 世界树核心
		/// </summary>
		public WorldTreeCore Core;

		/// <summary>
		/// 日志管理器
		/// </summary>
		public LogManager LogManager;

		/// <summary>
		/// 节点引用池管理器
		/// </summary>
		public ReferencedPoolManager ReferencedPoolManager;

		/// <summary>
		/// 全局法则执行器管理器
		/// </summary>
		public RuleBroadcastManager GlobalRuleExecutorManager;

		/// <summary>
		/// 世界时间管理器
		/// </summary>
		public WorldTimeManager WorldTimeManager;

		/// <summary>
		/// 世界之心
		/// </summary>
		public WorldHeartBase WorldHeart;

		/// <summary>
		/// 世界环境：线程上下文
		/// </summary>
		public WorldContext WorldContext;

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
			//初始化
			World = Line = this;
			Type = this.TypeToCode();
			//激活
			SetActive(true);
			//日志管理器
			this.Core.CreateCoreObject(out LogManager).Init(Id.ToString());

			//引用池管理器
			this.TryGraftComponent(this.PoolGetNode(out ReferencedPoolManager));
			//嫁接节点需要手动激活
			ReferencedPoolManager.SetActive(true);

			//全局法则执行器管理器
			this.AddComponent(out GlobalRuleExecutorManager);
			//世界时间管理器
			this.AddComponent(out WorldTimeManager);

			// 世界之心
			WorldHeart = NodeBranchHelper.AddNode(this, default(ComponentBranch), this.TypeToCode(heartType), heartType, out _, frameTime) as WorldHeartBase;
			// 世界之心启动
			WorldHeart.Run();
			// 世界环境
			this.AddComponent(out WorldContext);
			// 主世界生成
			WorldContext.Post(() =>
			{
				NodeBranchHelper.AddNode(this, default(ComponentBranch), this.TypeToCode(worldType), worldType, out _);
			});
		}

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
			Core.BeforeRemoveRuleGroup?.Send(this);

			//有严格的移除顺序
			this.RemoveAllTemp();
			WorldHeart?.Dispose();
			WorldContext?.Dispose();
			WorldTimeManager?.Dispose();
			GlobalRuleExecutorManager?.Dispose();
			ReferencedPoolManager?.Dispose();
			LogManager?.Dispose();
			MainWorld?.Dispose();
			this.RemoveAllComponent();
		}

		/// <summary>
		/// 框架释放
		/// </summary>
		public override void OnDispose()
		{
			ViewBuilder?.Dispose();
			ViewBuilder = null;
			SetActive(false);
		}

		#endregion

		#endregion

	}
}