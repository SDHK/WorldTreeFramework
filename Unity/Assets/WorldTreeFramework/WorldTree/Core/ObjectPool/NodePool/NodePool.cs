﻿/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/17 17:23

* 描述： 树节点对象池
*
* 管理类型： INode
*   
*   调用 节点 生命周期法则， 生成， 获取， 回收， 销毁，
*   
*   同时对 节点 赋予 根节点 和 Id 分发。
*   
*/
using System;

namespace WorldTree
{

    /// <summary>
    /// 实体对象池
    /// </summary>
    public class NodePool : GenericPool<INode>
        , ChildOf<PoolManagerBase<NodePool>>
    {
        public IRuleList<INewRule> newRule;
        public IRuleList<IGetRule> getRule;
        public IRuleList<IRecycleRule> recycleRule;
        public IRuleList<IDestroyRule> destroyRule;


        public NodePool() : base()
        {
            NewObject = ObjectNew;
            DestroyObject = ObjectDestroy;

            objectOnNew = ObjectOnNew;
            objectOnGet = ObjectOnGet;
            objectOnRecycle = ObjectOnRecycle;
            objectOnDestroy = ObjectOnDestroy;
        }

        public override string ToString()
        {
            return $"[NodePool<{ObjectType}>] : {Count} ";
        }

        /// <summary>
        /// 获取对象并转为指定类型
        /// </summary>
        public T Get<T>()
            where T : class
        {
            return Get() as T;
        }

        public override void Recycle(object obj) => Recycle(obj as INode);
        public void Recycle(INode obj)
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

        private INode ObjectNew(IPool pool)
        {
            INode obj = Activator.CreateInstance(ObjectType, true) as INode;
            obj.IsFromPool = true;
            obj.Core = Core;
            obj.Root = Core.Root;
            obj.Type = ObjectTypeCore;
            return obj;
        }
        private void ObjectDestroy(INode obj)
        {
        }

        private void ObjectOnNew(INode obj)
        {
            newRule?.Send(obj);
        }

        private void ObjectOnGet(INode obj)
        {
            obj.IsRecycle = false;
            getRule?.Send(obj);
        }

        public void ObjectOnRecycle(INode obj)
        {
            obj.IsRecycle = true;
            recycleRule?.Send(obj);
        }

        private void ObjectOnDestroy(INode obj)
        {
            obj.IsDisposed = true;
            destroyRule?.Send(obj);
        }

    }


    public static partial class NodePoolRule
    {
        class AddRule : AddRule<NodePool>
        {
            public override void OnEvent(NodePool self)
            {
                self.Core.RuleManager.SupportNodeRule(self.ObjectTypeCore);

                //生命周期法则
                self.newRule = self.Core.RuleManager.GetRuleList<INewRule>(self.ObjectTypeCore);
                self.getRule = self.Core.RuleManager.GetRuleList<IGetRule>(self.ObjectTypeCore);
                self.recycleRule = self.Core.RuleManager.GetRuleList<IRecycleRule>(self.ObjectTypeCore);
                self.destroyRule = self.Core.RuleManager.GetRuleList<IDestroyRule>(self.ObjectTypeCore);
            }
        }
        class DestroyRule : DestroyRule<NodePool>
        {
            public override void OnEvent(NodePool self)
            {
                self.DisposeAll();
                self.NewObject = null;
                self.DestroyObject = null;
                self.objectOnNew = null;
                self.objectOnGet = null;
                self.objectOnRecycle = null;
                self.objectOnDestroy = null;

                self.newRule = default;
                self.getRule = default;
                self.recycleRule = default;
                self.destroyRule = default;
            }
        }
    }
}
