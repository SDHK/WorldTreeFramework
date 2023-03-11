
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/18 9:35

* 描述： 世界树根
* 
* 框架的启动入口
* 
* 同时启动法则管理器和对象池管理器
* 
* 管理分发全局的节点与组件的生命周期
* 

*/

using System;

namespace WorldTree
{
    //剩余
    //异常处理？

    /// <summary>
    /// 世界树根
    /// </summary>
    public class WorldTreeRoot : Node
    {
        public UnitDictionary<long, INode> allEntity = new UnitDictionary<long, INode>();

        private RuleGroup AddRuleGroup;
        private RuleGroup RemoveRuleGroup;
        private RuleGroup EnableRuleGroup;
        private RuleGroup DisableRuleGroup;

        private RuleGroup NewRuleGroup;
        private RuleGroup GetRuleGroup;
        private RuleGroup RecycleRuleGroup;
        private RuleGroup DestroyRuleGroup;

        public IdManager IdManager;
        public RuleManager RuleManager;
        public UnitPoolManager UnitPoolManager;
        public NodePoolManager NodePoolManager;
        public StaticListenerRuleActuatorManager StaticListenerRuleActuatorManager;
        public DynamicListenerRuleActuatorManager DynamicListenerRuleActuatorManager;

        public WorldTreeRoot() : base()
        {
            //框架运转的核心组件
            IdManager = new IdManager();
            RuleManager = new RuleManager();
            UnitPoolManager = new UnitPoolManager();
            NodePoolManager = new NodePoolManager();
            StaticListenerRuleActuatorManager = new StaticListenerRuleActuatorManager();
            DynamicListenerRuleActuatorManager = new DynamicListenerRuleActuatorManager();

            //赋予根节点
            Root = this;
            IdManager.Root = this;
            RuleManager.Root = this;
            UnitPoolManager.Root = this;
            NodePoolManager.Root = this;
            StaticListenerRuleActuatorManager.Root = this;
            DynamicListenerRuleActuatorManager.Root = this;

            //赋予id
            Root.Id = IdManager.GetId();
            IdManager.Id = IdManager.GetId();
            RuleManager.Id = IdManager.GetId();
            UnitPoolManager.Id = IdManager.GetId();
            NodePoolManager.Id = IdManager.GetId();
            StaticListenerRuleActuatorManager.Id = IdManager.GetId();
            DynamicListenerRuleActuatorManager.Id = IdManager.GetId();


            //法则集合获取
            AddRuleGroup = Root.RuleManager.GetRuleGroup<IAddRule>();
            RemoveRuleGroup = Root.RuleManager.GetRuleGroup<IRemoveRule>();
            EnableRuleGroup = Root.RuleManager.GetRuleGroup<IEnableRule>();
            DisableRuleGroup = Root.RuleManager.GetRuleGroup<IDisableRule>();

            NewRuleGroup = Root.RuleManager.GetRuleGroup<INewRule>();
            GetRuleGroup = Root.RuleManager.GetRuleGroup<IGetRule>();
            RecycleRuleGroup = Root.RuleManager.GetRuleGroup<IRecycleRule>();
            DestroyRuleGroup = Root.RuleManager.GetRuleGroup<IDestroyRule>();

            //激活自己
            this.SetActive(true);

            //核心组件添加
            this.AddComponent(IdManager);
            this.AddComponent(RuleManager);
            this.AddComponent(UnitPoolManager);
            this.AddComponent(NodePoolManager);
            this.AddComponent(StaticListenerRuleActuatorManager);
            this.AddComponent(DynamicListenerRuleActuatorManager);

        }
        /// <summary>
        /// 释放
        /// </summary>
        public override void Dispose()
        {
            this.RemoveAll();
            allEntity.Clear();
        }


        /// <summary>
        /// 从池中获取单位对象
        /// </summary>
        public T GetUnit<T>()
        where T : class, IUnitPoolEventItem
        {
            Type type = typeof(T);
            if (UnitPoolManager is null)
            {
                T obj = Activator.CreateInstance(type, true) as T;
                obj.OnNew();
                obj.OnGet();
                return obj;
            }
            else
            {
                return UnitPoolManager.Get(type) as T;
            }
        }
        /// <summary>
        /// 从池中获取节点对象
        /// </summary>
        public T GetNode<T>() where T : class,INode => GetNode(typeof(T)) as T;

        /// <summary>
        /// 从池中获取节点对象
        /// </summary>
        public INode GetNode(Type type)
        {
            if (NodePoolManager is null)
            {
                INode obj = Activator.CreateInstance(type, true) as INode;
                obj.Id = IdManager.GetId();
                obj.Root = this;
                NewRuleGroup?.Send(obj);
                GetRuleGroup?.Send(obj);
                return obj;
            }
            else
            {
                return NodePoolManager.Get(type) as INode;
            }
        }

        /// <summary>
        /// 回收单位
        /// </summary>
        public void Recycle(IUnitPoolEventItem obj)
        {
            if (UnitPoolManager is null)
            {
                obj.IsRecycle = true;
                obj.OnRecycle();
                obj.IsDisposed = true;
                obj.OnDispose();
            }
            else
            {
                UnitPoolManager.Recycle(obj);
            }

        }

        /// <summary>
        /// 回收节点
        /// </summary>
        public void Recycle(INode obj)
        {
            if (NodePoolManager is null)
            {
                obj.IsRecycle = true;
                RecycleRuleGroup?.Send(obj);
                obj.IsDisposed = true;
                DestroyRuleGroup?.Send(obj);
            }
            else
            {
                NodePoolManager.Recycle(obj);
            }
        }



        public void Add(INode entity)
        {
            //广播给全部监听器!!!!
            entity.TrySendStaticListener<IListenerAddRule>();
            entity.TrySendDynamicListener<IListenerAddRule>();

            allEntity.TryAdd(entity.Id, entity);

            //这个节点的添加事件
            AddRuleGroup?.Send(entity);

            //检测添加静态监听
            StaticListenerRuleActuatorManager.TryAddListener(entity);
            //检测添加动态监听
            entity.ListenerSwitchesTarget(typeof(INode), ListenerState.Node);


            entity.SetActive(true);
            EnableRuleGroup?.Send(entity);//添加后调用激活事件

        }


        public void Remove(INode entity)
        {
            entity.SetActive(false);//激活标记变更

            entity.RemoveAll();//移除所有子节点和组件
            DisableRuleGroup?.Send(entity);//调用禁用事件

            //检测移除静态监听
            StaticListenerRuleActuatorManager.RemoveListener(entity);
            //检测移除动态监听
            entity.ListenerClearTarget();

            //这个节点的移除事件
            RemoveRuleGroup?.Send(entity);

            allEntity.Remove(entity.Id);

            //广播给全部监听器!!!!
            entity.TrySendStaticListener<IListenerRemoveRule>();
            entity.TrySendDynamicListener<IListenerRemoveRule>();

        }
    }
}
