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

namespace WorldTree
{
	//剩余
	//异常处理？


	//对法则执行器进行更加详细的划分，生命周期，全局事件，回调事件？

	//对象池需要一个启动标记？

	//Log改为 Self.Log



	//新增TimeUpdate,特化的双方法法则
	//真实与游戏时间双法则

	//真实时间和游戏时间

	//生命周期整理

	//执行器整理，改为用结构体指针

	//时域与时间轮

	//将Add综合到核心



	/// <summary>
	/// 世界树核心
	/// </summary>
	public class WorldTreeCore : CoreNode
	{
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


		public WorldTreeCore()
		{
			this.Awake();
		} 

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
			self.GraftComponent(self.Root = self.PoolGet<WorldTreeRoot>());

			self.ReferencedPoolManager.SetActive(true);
			self.IdManager.SetActive(true);
			self.RuleManager.SetActive(true);
			self.Root.SetActive(true);

			//游戏时间管理器
			//self.AddComponent(out self.GameTimeManager);

			//核心激活
			self.SetActive(true);

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
		}
		#endregion

		/// <summary>
		/// 框架刷新
		/// </summary>
		public static void Update()
		{
		}

		#region 节点添加与移除

		///// <summary>
		///// 核心添加一个节点
		///// </summary>
		//public static void AddNode(this WorldTreeCore self, INode node)
		//{
		//	self.ReferencedPoolManager.TryAdd(node);

		//	node.SetActive(true);
		//	self.EnableRuleGroup?.Send(node);//添加后调用激活事件

		//	//广播给全部监听器!!!
		//	if (node is not ICoreNode)
		//	{
		//		node.GetListenerActuator<IListenerAddRule>()?.Send(node);
		//	}

		//	if (node is INodeListener nodeListener && node is not ICoreNode)
		//	{
		//		//检测添加静态监听
		//		self.ReferencedPoolManager.TryAddStaticListener(nodeListener);
		//	}

		//	//这个节点的添加事件
		//	self.AddRuleGroup?.Send(node);
		//}

		///// <summary>
		///// 核心移除一个节点
		///// </summary>
		//public static void RemoveNode(this WorldTreeCore self, INode node)
		//{
		//	//从父节点中移除
		//	node.RemoveInParent();
		//	//引用关系移除通知
		//	node.SendAllReferencedNodeRemove();

		//	node.SetActive(false);//激活标记变更

		//	self.DisableRuleGroup?.Send(node);//调用禁用事件

		//	if (node is INodeListener && node is not ICoreNode)
		//	{
		//		INodeListener nodeListener = (node as INodeListener);

		//		//检测移除静态监听
		//		self.ReferencedPoolManager.RemoveStaticListener(nodeListener);
		//		//检测移除动态监听
		//		self.ReferencedPoolManager.RemoveDynamicListener(nodeListener);
		//	}

		//	//这个节点的移除事件
		//	self.RemoveRuleGroup?.Send(node);

		//	//广播给全部监听器!!!
		//	if (node is not ICoreNode)
		//	{
		//		node.GetListenerActuator<IListenerRemoveRule>()?.Send(node);
		//	}

		//	self.ReferencedPoolManager.Remove(node);

		//	node.DisposeDomain();//清除域节点
		//	node.Parent = null;//清除父节点
		//}
		#endregion
	}
}
