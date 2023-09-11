
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
    //剩余
    //异常处理？


    //对法则执行器进行更加详细的划分，生命周期，全局事件，回调事件？

    //对象池需要一个启动标记？

    //Log改为 Self.Log

    //将Add综合到核心??


    //新增TimeUpdate,特化的双方法法则
    //真实与游戏时间双法则

    //真实时间和游戏时间

    //生命周期整理

    /*
        New
        Get
        Awake
        Enable
        Add
    */
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


    public static class WorldTreeCoreRule
    {

        #region 框架启动与销毁




        /// <summary>
        /// 框架启动
        /// </summary>
        public static void Awake(this WorldTreeCore self)
        {
            //根节点初始化
            self.Type = TypeInfo<WorldTreeCore>.HashCode64;
            self.Core = self;
            self.Branch = self;

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
            self.AddComponent(self.ReferencedPoolManager);
            self.AddComponent(self.IdManager);
            self.AddComponent(self.RuleManager);

            //对象池组件。 out 会在执行完之前就赋值 ，但这时候对象池并没有准备好
            self.UnitPoolManager = self.AddNewComponent(out UnitPoolManager _);
            self.NodePoolManager = self.AddNewComponent(out NodePoolManager _);
            self.ArrayPoolManager = self.AddNewComponent(out ArrayPoolManager _);

            //树根节点
            self.AddComponent(self.Root = self.PoolGet<WorldTreeRoot>());

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
            self.RemoveComponent<ArrayPoolManager>();
            self.RemoveComponent<NodePoolManager>();
            self.RemoveComponent<UnitPoolManager>();
            self.RemoveComponent<RuleManager>();
            self.RemoveComponent<RealTimeManager>();
            self.RemoveComponent<IdManager>();
            self.RemoveComponent<ReferencedPoolManager>();

            self.RemoveAll();

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

        #region 对象获取与回收

        #region Node

        /// <summary>
        /// 新建节点对象
        /// </summary>
        /// <remarks>不执行法则生命周期</remarks>
        private static T NewNode<T>(this INode self, out T node) where T : class, INode
        {
            Type type = typeof(T);
            node = Activator.CreateInstance(type, true) as T;
            node.Type = TypeInfo<T>.HashCode64;
            node.Core = self.Core;
            node.Root = self.Root;
            node.Id = self.Core.IdManager.GetId();
            return node;
        }

        /// <summary>
        /// 新建节点对象
        /// </summary>
        /// <remarks>不执行法则生命周期</remarks>
        private static INode NewNode(this INode self, long type)
        {
            INode node = Activator.CreateInstance(type.HashCore64ToType(), true) as INode;
            node.Type = type;
            node.Core = self.Core;
            node.Root = self.Root;
            node.Id = self.Core.IdManager.GetId();
            return node;
        }

        /// <summary>
        /// 新建节点对象并调用生命周期
        /// </summary>
        /// <remarks>执行法则生命周期</remarks>
        public static T NewNodeLifecycle<T>(this WorldTreeCore self, out T node) where T : class, INode
        {
            self.NewNode(out node);
            self.RuleManager.SupportNodeRule(node.Type);
            self.NewRuleGroup?.Send(node);
            self.GetRuleGroup?.Send(node);
            return node;
        }

        /// <summary>
        /// 新建节点对象并调用生命周期
        /// </summary>
        /// <remarks>执行法则生命周期</remarks>
        public static INode NewNodeLifecycle(this WorldTreeCore self, long type)
        {
            INode node = self.NewNode(type);
            self.RuleManager.SupportNodeRule(node.Type);
            self.NewRuleGroup?.Send(node);
            self.GetRuleGroup?.Send(node);
            return node;
        }


        /// <summary>
        /// 从池中获取节点对象
        /// </summary>
        public static T GetNode<T>(this WorldTreeCore self) where T : class, INode
        {

            if (self.IsActive)
            {
                if (self.NodePoolManager.TryGet(TypeInfo<T>.HashCode64, out INode node))
                {
                    node.Id = self.IdManager.GetId();
                    return node as T;
                }
            }
            return self.NewNodeLifecycle<T>(out _);
        }

        /// <summary>
        /// 从池中获取节点对象
        /// </summary>
        public static INode GetNode(this WorldTreeCore self, long type)
        {
            if (self.IsActive)
            {
                if (self.NodePoolManager.TryGet(type, out INode node))
                {
                    node.Id = self.IdManager.GetId();
                    return node;
                }
            }
            return self.NewNodeLifecycle(type);
        }

        /// <summary>
        /// 回收节点
        /// </summary>
        public static void Recycle(this WorldTreeCore self, INode obj)
        {
            if (self.IsActive && obj.IsFromPool)
            {
                if (self.NodePoolManager.TryRecycle(obj)) return;
            }
            obj.IsRecycle = true;
            self.RecycleRuleGroup?.Send(obj);
            obj.IsDisposed = true;
            self.DestroyRuleGroup?.Send(obj);
        }


        #endregion

        #region Unit

        /// <summary>
        /// 从池中获取单位对象
        /// </summary>
        public static T GetUnit<T>(this WorldTreeCore self)
        where T : class, IUnitPoolEventItem
        {
            if (self.IsActive)
            {
                if (self.UnitPoolManager.TryGet(TypeInfo<T>.HashCode64, out IUnitPoolEventItem unit))
                {
                    return unit as T;
                }
            }
            T obj = Activator.CreateInstance(TypeInfo<T>.Type, true) as T;
            obj.Type = TypeInfo<T>.HashCode64;
            obj.OnNew();
            obj.OnGet();
            return obj;
        }

        /// <summary>
        /// 回收单位
        /// </summary>
        public static void Recycle(this WorldTreeCore self, IUnitPoolEventItem obj)
        {
            if (self.IsActive && obj.IsFromPool)
            {
                if (self.UnitPoolManager.TryRecycle(obj)) return;
            }
            obj.IsRecycle = true;
            obj.OnRecycle();
            obj.IsDisposed = true;
            obj.OnDispose();
        }


        #endregion

        #region Array

        /// <summary>
        /// 获取数组对象
        /// </summary>
        public static T[] GetArray<T>(this WorldTreeCore self, out T[] outT, int Length)
        {
            Type type = typeof(T);
            if (self.ArrayPoolManager != null)
            {
                outT = self.ArrayPoolManager.Get(type, Length) as T[];
            }
            else
            {
                outT = Array.CreateInstance(type, Length) as T[];
            }
            return outT;
        }

        /// <summary>
        /// 获取数组对象
        /// </summary>
        public static T[] GetArray<T>(this WorldTreeCore self, int Length)
        {
            Type type = typeof(T);
            if (self.ArrayPoolManager != null)
            {
                return self.ArrayPoolManager.Get(type, Length) as T[];
            }
            return Array.CreateInstance(type, Length) as T[];
        }

        /// <summary>
        /// 回收数组
        /// </summary>
        public static void Recycle(this WorldTreeCore self, Array obj)
        {
            if (self.ArrayPoolManager != null)
            {
                self.ArrayPoolManager.Recycle(obj);
            }
            else
            {
                Array.Clear(obj, 0, obj.Length);
            }
        }


        #endregion


        #endregion

        #region 节点添加与移除
        /// <summary>
        /// 核心添加一个节点
        /// </summary>
        public static void AddNode(this WorldTreeCore self, INode node)
        {
            self.ReferencedPoolManager.TryAdd(node);

            node.SetActive(true);
            self.EnableRuleGroup?.Send(node);//添加后调用激活事件

            //广播给全部监听器!!!
            if (node is not ICoreNode)
            {
                node.GetListenerActuator<IListenerAddRule>()?.Send(node);
            }

            if (node is INodeListener nodeListener && node is not ICoreNode)
            {
                //检测添加静态监听
                self.ReferencedPoolManager.TryAddStaticListener(nodeListener);
            }

            //这个节点的添加事件
            self.AddRuleGroup?.Send(node);
        }

        /// <summary>
        /// 核心移除一个节点
        /// </summary>
        public static void RemoveNode(this WorldTreeCore self, INode node)
        {
            //从父节点中移除
            node.RemoveInParent();
            //引用关系移除通知
            node.SendAllReferencedNodeRemove();

            node.SetActive(false);//激活标记变更

            self.DisableRuleGroup?.Send(node);//调用禁用事件

            if (node is INodeListener && node is not ICoreNode)
            {
                INodeListener nodeListener = (node as INodeListener);

                //检测移除静态监听
                self.ReferencedPoolManager.RemoveStaticListener(nodeListener);
                //检测移除动态监听
                self.ReferencedPoolManager.RemoveDynamicListener(nodeListener);
            }

            //这个节点的移除事件
            self.RemoveRuleGroup?.Send(node);

            //广播给全部监听器!!!
            if (node is not ICoreNode)
            {
                node.GetListenerActuator<IListenerRemoveRule>()?.Send(node);
            }

            self.ReferencedPoolManager.Remove(node);

            node.DisposeDomain();//清除域节点
            node.Parent = null;//清除父节点
        }
        #endregion
    }
}
