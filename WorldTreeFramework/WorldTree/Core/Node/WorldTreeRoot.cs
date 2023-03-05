
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
    //对Node底层再抽一层接口

    /// <summary>
    /// 世界树根
    /// </summary>
    public class WorldTreeRoot : Node
    {
        public UnitDictionary<long, Node> allEntity = new UnitDictionary<long, Node>();

        private RuleGroup addSystems;
        private RuleGroup removeSystems;
        private RuleGroup enableSystems;
        private RuleGroup disableSystems;


        public IdManager IdManager;
        public RuleManager RuleManager;
        public UnitPoolManager UnitPoolManager;
        public EntityPoolManager EntityPoolManager;
        public StaticListenerBroadcastManager StaticListenerBroadcastManager;
        public DynamicListenerBroadcastManager DynamicListenerBroadcastManager;

        public WorldTreeRoot() : base()
        {
            //此时没有对象池，直接新建容器
            Children = new UnitDictionary<long, Node>();
            Components = new UnitDictionary<Type, Node>();

            //框架运转的核心组件
            IdManager = new IdManager();
            RuleManager = new RuleManager();
            UnitPoolManager = new UnitPoolManager();
            EntityPoolManager = new EntityPoolManager();
            StaticListenerBroadcastManager = new StaticListenerBroadcastManager();
            DynamicListenerBroadcastManager = new DynamicListenerBroadcastManager();

            //赋予根节点
            Root = this;
            IdManager.Root = this;
            RuleManager.Root = this;
            UnitPoolManager.Root = this;
            EntityPoolManager.Root = this;
            StaticListenerBroadcastManager.Root = this;
            DynamicListenerBroadcastManager.Root = this;

            //赋予id
            Root.id = IdManager.GetId();
            IdManager.id = IdManager.GetId();
            RuleManager.id = IdManager.GetId();
            UnitPoolManager.id = IdManager.GetId();
            EntityPoolManager.id = IdManager.GetId();
            StaticListenerBroadcastManager.id = IdManager.GetId();
            DynamicListenerBroadcastManager.id = IdManager.GetId();

            //实体管理器系统事件获取
            addSystems = Root.RuleManager.GetRuleGroup<IAddRule>();
            removeSystems = Root.RuleManager.GetRuleGroup<IRemoveRule>();
            enableSystems = Root.RuleManager.GetRuleGroup<IEnableRule>();
            disableSystems = Root.RuleManager.GetRuleGroup<IDisableRule>();

            //激活自己
            SetActive(true);

            //核心组件添加
            AddComponent(IdManager);
            AddComponent(RuleManager);
            AddComponent(UnitPoolManager);
            AddComponent(EntityPoolManager);
            AddComponent(StaticListenerBroadcastManager);
            AddComponent(DynamicListenerBroadcastManager);

        }
        /// <summary>
        /// 释放
        /// </summary>
        public override void Dispose()
        {
            RemoveAll();
            allEntity.Clear();
        }



        public void Add(Node entity)
        {
            //广播给全部监听器!!!!
            entity.TrySendStaticListener<IListenerAddSystem>();
            entity.TrySendDynamicListener<IListenerAddSystem>();

            allEntity.TryAdd(entity.id, entity);

            //这个实体的添加事件
            addSystems?.Send(entity);

            //检测添加静态监听
            StaticListenerBroadcastManager.TryAddListener(entity);
            //检测添加动态监听
            entity.ListenerSwitchesTarget(typeof(Node), ListenerState.Node);


            entity.SetActive(true);
            enableSystems?.Send(entity);//添加后调用激活事件

        }


        public void Remove(Node entity)
        {
            entity.SetActive(false);//激活标记变更

            entity.RemoveAll();//移除所有子节点和组件
            disableSystems?.Send(entity);//调用禁用事件

            //检测移除静态监听
            StaticListenerBroadcastManager.RemoveListener(entity);
            //检测移除动态监听
            entity.ListenerClearTarget();

            //这个实体的移除事件
            removeSystems?.Send(entity);

            allEntity.Remove(entity.id);

            //广播给全部监听器!!!!
            entity.TrySendStaticListener<IListenerRemoveSystem>();
            entity.TrySendDynamicListener<IListenerRemoveSystem>();

        }
    }
}
