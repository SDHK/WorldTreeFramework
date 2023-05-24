
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
    //TreeTween
    //TreeTask设计问题
    //RuleActuator 或 DynamicNodeQueue 大改

    /// <summary>
    /// 世界树核心
    /// </summary>
    public class WorldTreeCore : Node
    {
        /// <summary>
        /// 全部节点
        /// </summary>
        public UnitDictionary<long, INode> AllNode = new UnitDictionary<long, INode>();

        public IRuleGroup<IAddRule> AddRuleGroup;
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
        /// 静态监听器法则执行器管理器
        /// </summary>
        public StaticListenerRuleActuatorManager StaticListenerRuleActuatorManager;
        /// <summary>
        /// 动态监听器法则执行器管理器
        /// </summary>
        public DynamicListenerRuleActuatorManager DynamicListenerRuleActuatorManager;

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
            self.Type = self.GetType();
            self.Core = self;
            self.Branch = self;

            //框架核心启动组件新建
            self.IdManager = new IdManager();
            self.RuleManager = new RuleManager();

            //Id
            self.Core.Id = self.IdManager.GetId();
            self.IdManager.Id = self.IdManager.GetId();
            self.RuleManager.Id = self.IdManager.GetId();

            //核心
            self.IdManager.Core = self;
            self.RuleManager.Core = self;

            //生命周期法则
            self.NewRuleGroup = self.RuleManager.GetRuleGroup<INewRule>();
            self.GetRuleGroup = self.RuleManager.GetRuleGroup<IGetRule>();
            self.RecycleRuleGroup = self.RuleManager.GetRuleGroup<IRecycleRule>();
            self.DestroyRuleGroup = self.RuleManager.GetRuleGroup<IDestroyRule>();

            self.AddRuleGroup = self.RuleManager.GetRuleGroup<IAddRule>();
            self.RemoveRuleGroup = self.RuleManager.GetRuleGroup<IRemoveRule>();
            self.EnableRuleGroup = self.RuleManager.GetRuleGroup<IEnableRule>();
            self.DisableRuleGroup = self.RuleManager.GetRuleGroup<IDisableRule>();

            //核心组件 id与法则
            self.AddComponent(self.IdManager);
            self.AddComponent(self.RuleManager);

            //对象池组件。 out 会在执行完之前就赋值 ，但这时候对象池并没有准备好
            self.UnitPoolManager = self.AddComponent(out UnitPoolManager _);
            self.NodePoolManager = self.AddComponent(out NodePoolManager _);
            self.AddComponent(out self.ArrayPoolManager);

            //监听器组件
            self.AddComponent(out self.StaticListenerRuleActuatorManager);
            self.AddComponent(out self.DynamicListenerRuleActuatorManager);


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
            self.RemoveAll();
            self.AllNode.Clear();
            self.AllNode = default;

            self.NewRuleGroup = default;
            self.GetRuleGroup = default;
            self.RecycleRuleGroup = default;
            self.DestroyRuleGroup = default;

            self.AddRuleGroup = default;
            self.RemoveRuleGroup = default;
            self.EnableRuleGroup = default;
            self.DisableRuleGroup = default;

            self.IdManager = default;
            self.RuleManager = default;
            self.UnitPoolManager = default;
            self.NodePoolManager = default;
            self.ArrayPoolManager = default;
            self.StaticListenerRuleActuatorManager = default;
            self.DynamicListenerRuleActuatorManager = default;

        }
        #endregion

        #region 对象获取与回收

        #region Node

        /// <summary>
        /// 从池中获取节点对象
        /// </summary>
        public static T GetNode<T>(this WorldTreeCore self) where T : class, INode => self.GetNode(typeof(T)) as T;

        /// <summary>
        /// 从池中获取节点对象
        /// </summary>
        public static INode GetNode(this WorldTreeCore self, Type type)
        {
            if (self.NodePoolManager != null)
            {
                if (!self.NodePoolManager.IsRecycle)
                {
                    return self.NodePoolManager.Get(type) as INode;
                }
            }

            INode obj = Activator.CreateInstance(type, true) as INode;
            obj.Id = self.IdManager.GetId();
            obj.Core = self;
            obj.Root = self.Root;
            obj.Type = type;

            self.NewRuleGroup?.Send(obj);
            self.GetRuleGroup?.Send(obj);
            return obj;
        }

        /// <summary>
        /// 回收节点
        /// </summary>
        public static void Recycle(this WorldTreeCore self, INode obj)
        {
            if (self.NodePoolManager != null)
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
            Type type = typeof(T);
            if (self.UnitPoolManager != null)
            {
                if (!self.UnitPoolManager.IsRecycle)
                {
                    return self.UnitPoolManager.Get(type) as T;
                }
            }

            T obj = Activator.CreateInstance(type, true) as T;
            obj.OnNew();
            obj.OnGet();
            return obj;
        }

        /// <summary>
        /// 回收单位
        /// </summary>
        public static void Recycle(this WorldTreeCore self, IUnitPoolEventItem obj)
        {
            if (self.UnitPoolManager != null)
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
            node.SetActive(true);
            self.EnableRuleGroup?.Send(node);//添加后调用激活事件

            //广播给全部监听器!!!!
            if (node.Branch.Id != self.Id)
            {
                node.TrySendStaticListener<IListenerAddRule>();
                node.TrySendDynamicListener<IListenerAddRule>();
            }

            self.AllNode.TryAdd(node.Id, node);

            //这个节点的添加事件
            self.AddRuleGroup?.Send(node);


            if (node is INodeListener && node.Branch.Id != self.Id)
            {
                INodeListener nodeListener = (node as INodeListener);
                //检测添加静态监听
                self.StaticListenerRuleActuatorManager.TryAddListener(nodeListener);
                //检测添加动态监听
                nodeListener.ListenerSwitchesTarget(typeof(INode), ListenerState.Node);
            }


        }

        /// <summary>
        /// 核心移除一个节点
        /// </summary>
        public static void RemoveNode(this WorldTreeCore self, INode node)
        {

            //引用关系移除通知
            node.SendAllReferencedNodeRemove();

            node.SetActive(false);//激活标记变更

            node.RemoveAll();//移除所有子节点和组件
            self.DisableRuleGroup?.Send(node);//调用禁用事件

            if (node is INodeListener && node.Branch.Id != self.Id)
            {
                INodeListener nodeListener = (node as INodeListener);

                //检测移除静态监听
                self.StaticListenerRuleActuatorManager.RemoveListener(nodeListener);
                //检测移除动态监听
                nodeListener.ListenerClearTarget();
            }

            //这个节点的移除事件
            self.RemoveRuleGroup?.Send(node);


            //广播给全部监听器!!!!
            if (node.Branch.Id != self.Id)
            {
                //检测移除静态监听
                node.TrySendStaticListener<IListenerRemoveRule>();

                //检测移除动态监听
                node.TrySendDynamicListener<IListenerRemoveRule>();
            }

            self.AllNode.Remove(node.Id);
        }
        #endregion
    }
}
