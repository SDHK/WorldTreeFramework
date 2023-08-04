/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/6 14:29

* 描述： 子节点
* 
* 用节点ID作为键值，因此可以添加相同类型的子节点
*/

using System;

namespace WorldTree
{
    public static class NodeChildrenRule
    {
        /// <summary>
        /// 子节点
        /// </summary>
        public static UnitDictionary<long, INode> ChildrenDictionary(this INode self)
        {
            if (self.m_Children == null)
            {
                self.m_Children = self.PoolGet<UnitDictionary<long, INode>>();
            }
            return self.m_Children;
        }

        #region 获取

        /// <summary>
        /// 尝试通过id获取子节点
        /// </summary>
        public static bool TryGetChildren(this INode self, long id, out INode node)
        {
            if (self.m_Children == null)
            {
                node = null;
                return false;
            }
            else
            {
                return self.m_Children.TryGetValue(id, out node);
            }
        }


        #endregion

        #region 移除
        /// <summary>
        /// 移除子节点
        /// </summary>
        public static void RemoveChildren(this INode self, long id)
        {
            if (self.m_Children != null)
            {
                if (self.m_Children.TryGetValue(id, out INode node))
                {
                    node.Dispose();
                }
            }
        }

        /// <summary>
        /// 移除全部子节点
        /// </summary>
        public static void RemoveAllChildren(this INode self)
        {
            if (self.m_Children != null ? self.m_Children.Count != 0 : false)
            {
                var entitys = self.PoolGet<UnitStack<INode>>();
                foreach (var item in self.m_Children) entitys.Push(item.Value);

                int length = entitys.Count;
                for (int i = 0; i < length; i++)
                {
                    entitys.Pop().Dispose();
                }
                entitys.Dispose();
            }
        }
        #endregion
        #region 添加

        #region 替换或添加

        /// <summary>
        /// 添加野节点或替换父节点 （替换：从原父节点移除并接入新节点，会判断刷新活跃状态）
        /// </summary>
        public static void AddChild<N, T>(this N self, T node)
            where N : class, INode
            where T : class, INode, ChildOf<N>
        {
            if (node != null)
            {
                if (self.ChildrenDictionary().TryAdd(node.Id, node))
                {
                    if (node.Parent != null)
                    {
                        node.TraversalLevelDisposeDomain();
                        node.RemoveInParent();
                        if (node.Branch != node) node.Branch = self.Branch;
                        node.Parent = self;
                        node.isComponent = false;
                        node.RefreshActive();
                    }
                    else //野节点添加
                    {
                        if (node.Branch != node) node.Branch = self.Branch;
                        node.Parent = self;
                        node.isComponent = false;
                        node.TrySendRule<IAwakeRule>();
                        self.Core.AddNode(node);
                    }
                }
            }
        }
        #endregion

        #region 类型添加

        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public static INode AddChild(this INode self, Type type)
        {
            INode node = self.PoolGet(type);
            if (self.ChildrenDictionary().TryAdd(node.Id, node))
            {
                node.Parent = self;
                node.Branch = self.Branch;
                node.TrySendRule<IAwakeRule>();
                self.Core.AddNode(node);
            }
            return node;
        }

        #endregion

        /// <summary>
        /// 尝试添加子节点
        /// </summary>
        private static bool TryAddChild(this INode self, Type type, out INode node)
        {
            node = self.PoolGet(type);
            if (self.ChildrenDictionary().TryAdd(node.Id, node))
            {
                node.Branch = self.Branch;
                node.Parent = self;
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 尝试添加子节点
        /// </summary>
        private static bool TryAddChild<T>(this INode self, out T Node)
            where T : class, INode
        {
            var type = typeof(T);
            INode node = self.PoolGet(type);
            if (self.ChildrenDictionary().TryAdd(node.Id, node))
            {
                node.Branch = self.Branch;
                node.Parent = self;
                Node = node as T;
                return true;
            }
            else
            {
                Node = node as T;
                return false;
            }
        }

        /// <summary>
        /// 尝试添加子节点
        /// </summary>
        private static bool TryAddNewChild<T>(this INode self, out T Node)
            where T : class, INode
        {
            var type = typeof(T);
            INode node = self.Core.NewNodeLifecycle(type);
            if (self.ChildrenDictionary().TryAdd(node.Id, node))
            {
                node.Branch = self.Branch;
                node.Parent = self;
                Node = node as T;
                return true;
            }
            else
            {
                Node = node as T;
                return false;
            }
        }


        #region ChildOf

        public static T AddNewChild<N, T>(this N self, out T node)
            where N : class, INode
            where T : class, INode, ChildOf<N>, AsRule<IAwakeRule>
        {
            if (self.TryAddNewChild(out node))
            {
                node.SendRule(default(IAwakeRule));
                self.Core.AddNode(node);
            }
            return node;
        }


        //====================

        public static T AddChild<N, T>(this N self, out T node)
            where N : class, INode
            where T : class, INode, ChildOf<N>, AsRule<IAwakeRule>
        {
            if (self.TryAddChild(out node))
            {
                node.SendRule(default(IAwakeRule));
                self.Core.AddNode(node);
            }
            return node;
        }

        public static T AddChild<N, T, T1>(this N self, out T node, T1 arg1)
            where N : class, INode
            where T : class, INode, ChildOf<N>, AsRule<IAwakeRule<T1>>
        {
            if (self.TryAddChild(out node))
            {
                node.SendRule(default(IAwakeRule<T1>), arg1);
                self.Core.AddNode(node);
            }
            return node;
        }
        public static T AddChild<N, T, T1, T2>(this N self, out T node, T1 arg1, T2 arg2)
            where N : class, INode
            where T : class, INode, ChildOf<N>, AsRule<IAwakeRule<T1, T2>>
        {
            if (self.TryAddChild(out node))
            {
                node.SendRule(default(IAwakeRule<T1, T2>), arg1, arg2);
                self.Core.AddNode(node);
            }
            return node;
        }

        public static T AddChild<N, T, T1, T2, T3>(this N self, out T node, T1 arg1, T2 arg2, T3 arg3)
            where N : class, INode
            where T : class, INode, ChildOf<N>, AsRule<IAwakeRule<T1, T2, T3>>
        {
            if (self.TryAddChild(out node))
            {
                node.SendRule(default(IAwakeRule<T1, T2, T3>), arg1, arg2, arg3);
                self.Core.AddNode(node);
            }
            return node;
        }

        public static T AddChild<N, T, T1, T2, T3, T4>(this N self, out T node, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            where N : class, INode
            where T : class, INode, ChildOf<N>, AsRule<IAwakeRule<T1, T2, T3, T4>>
        {
            if (self.TryAddChild(out node))
            {
                node.SendRule(default(IAwakeRule<T1, T2, T3, T4>), arg1, arg2, arg3, arg4);
                self.Core.AddNode(node);
            }
            return node;
        }
        public static T AddChild<N, T, T1, T2, T3, T4, T5>(this N self, out T node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
            where N : class, INode
            where T : class, INode, ChildOf<N>, AsRule<IAwakeRule<T1, T2, T3, T4, T5>>
        {
            if (self.TryAddChild(out node))
            {
                node.SendRule(default(IAwakeRule<T1, T2, T3, T4, T5>), arg1, arg2, arg3, arg4, arg5);
                self.Core.AddNode(node);
            }
            return node;
        }

        #endregion

        #endregion
    }
}
