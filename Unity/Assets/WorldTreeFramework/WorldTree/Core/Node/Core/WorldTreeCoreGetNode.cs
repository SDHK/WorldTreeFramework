
/****************************************

* 作者： 闪电黑客
* 日期： 2023/9/11 20:42

* 描述： 

*/

using System;

namespace WorldTree
{
    public static partial class WorldTreeCoreRule
    {
        /// <summary>
        /// 新建节点对象
        /// </summary>
        /// <remarks>不执行法则生命周期</remarks>
        private static T NewNode<T>(this INode self, out T node) where T : class, INode
        {
            Type type = typeof(T);
            node = Activator.CreateInstance(type, true) as T;
            node.Type = TypeInfo<T>.TypeCode;
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
                if (self.NodePoolManager.TryGet(TypeInfo<T>.TypeCode, out INode node))
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
    }
}
