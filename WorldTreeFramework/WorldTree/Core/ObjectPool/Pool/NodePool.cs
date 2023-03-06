/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/17 17:23

* 描述： 树节点对象池
*
* 管理类型： Node
*   
*   调用 节点 生命周期法则， 生成， 获取， 回收， 销毁，
*   
*   同时对 节点 赋予 根节点 和 Id 分发。
*   
*/
using System;
using System.Collections.Generic;

namespace WorldTree
{

    class NodePoolAddRule : AddRule<NodePool>
    {
        public override void OnEvent(NodePool self)
        {
            //生命周期法则
            self.newRule = self.GetRuleList<INewRule>(self.ObjectType);
            self.getRule = self.GetRuleList<IGetRule>(self.ObjectType);
            self.recycleRule = self.GetRuleList<IRecycleRule>(self.ObjectType);
            self.destroyRule = self.GetRuleList<IDestroyRule>(self.ObjectType);
        }
    }


    /// <summary>
    /// 实体对象池
    /// </summary>
    public class NodePool : GenericPool<Node>
    {
        public List<IRule> newRule;
        public List<IRule> getRule;
        public List<IRule> recycleRule;
        public List<IRule> destroyRule;

        /// <summary>
        /// 引用池
        /// </summary>
        public Dictionary<long, Node> Nodes;

        public NodePool(Type type) : base()
        {

            ObjectType = type;

            Nodes = new Dictionary<long, Node>();

            NewObject = ObjectNew;
            DestroyObject = ObjectDestroy;

            objectOnNew = ObjectOnNew;
            objectOnGet = ObjectOnGet;
            objectOnRecycle = ObjectOnRecycle;
            objectOnDestroy = ObjectOnDestroy;

        }

        /// <summary>
        /// 获取对象并转为指定类型
        /// </summary>
        public T Get<T>()
            where T : class
        {
            return Get() as T;
        }

        public override string ToString()
        {
            return $"[NodePool<{ObjectType}>] : {Count} ";
        }

        private Node ObjectNew(IPool pool)
        {
            Node obj = Activator.CreateInstance(ObjectType, true) as Node;
            obj.thisPool = this;
            obj.id = Root.IdManager.GetId();
            obj.Root = Root;
            return obj;
        }
        public override void Recycle(object obj) => Recycle(obj as Node);
        public void Recycle(Node obj)
        {
            lock (objetPool)
            {
                if (obj != null)
                {
                    if (maxLimit == -1 || objetPool.Count < maxLimit)
                    {
                        if (obj.IsRecycle) return;

                        objectOnRecycle.Invoke(obj);
                        objetPool.Enqueue(obj);
                    }
                    else
                    {
                        objectOnRecycle.Invoke(obj);
                        objectOnDestroy.Invoke(obj);
                        DestroyObject.Invoke(obj);
                    }
                }
            }
        }
        private void ObjectDestroy(Node obj)
        {
            Root.IdManager.Recycle(obj.id);
        }

        private void ObjectOnNew(Node obj)
        {
            newRule?.Send(obj);
        }

        private void ObjectOnGet(Node obj)
        {
            obj.IsRecycle = false;
            Nodes.TryAdd(obj.id, obj);
            getRule?.Send(obj);
        }

        private void ObjectOnRecycle(Node obj)
        {
            obj.IsRecycle = true;
            recycleRule?.Send(obj);
            Nodes.Remove(obj.id);
        }

        private void ObjectOnDestroy(Node obj)
        {
            obj.IsDisposed = true;
            destroyRule?.Send(obj);
        }

    }
}
