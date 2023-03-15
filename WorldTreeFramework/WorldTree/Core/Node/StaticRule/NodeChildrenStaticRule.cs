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
    public static class NodeChildrenStaticRule
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
        /// 通过id获取子节点
        /// </summary>
        public static INode GetChildren(this INode self, long id)
        {
            if (self.m_Children == null)
            {
                return null;
            }
            else
            {
                if (self.m_Children.TryGetValue(id, out INode node))
                {
                    return node;
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 通过id获取子节点
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
        public static void AddChildren(this INode self, INode node)
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
                        node.SendRule<IAwakeRule>();
                        self.Core.Add(node);
                    }
                }
            }
        }
        #endregion

        #region 类型添加

        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public static INode AddChildren(this INode self, Type type)
        {
            INode node = self.PoolGet(type);
            if (self.ChildrenDictionary().TryAdd(node.Id, node))
            {
                node.Parent = self;
                node.Branch = self.Branch;
                node.SendRule<IAwakeRule>();
                self.Core.Add(node);
            }
            return node;
        }
        #endregion

        #region 泛型添加



        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public static T AddChildren<T>(this INode self, out T node) where T : class, INode => node = self.AddChildren<T>();
        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public static T AddChildren<T, T1>(this INode self, out T node, T1 arg1) where T : class, INode => node = self.AddChildren<T, T1>(arg1);
        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public static T AddChildren<T, T1, T2>(this INode self, out T node, T1 arg1, T2 arg2) where T : class, INode => node = self.AddChildren<T, T1, T2>(arg1, arg2);
        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public static T AddChildren<T, T1, T2, T3>(this INode self, out T node, T1 arg1, T2 arg2, T3 arg3) where T : class, INode => node = self.AddChildren<T, T1, T2, T3>(arg1, arg2, arg3);
        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public static T AddChildren<T, T1, T2, T3, T4>(this INode self, out T node, T1 arg1, T2 arg2, T3 arg3, T4 arg4) where T : class, INode => node = self.AddChildren<T, T1, T2, T3, T4>(arg1, arg2, arg3, arg4);
        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public static T AddChildren<T, T1, T2, T3, T4, T5>(this INode self, out T node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) where T : class, INode => node = self.AddChildren<T, T1, T2, T3, T4, T5>(arg1, arg2, arg3, arg4, arg5);

        /// <summary>
        /// 尝试添加子节点
        /// </summary>
        public static bool TryAddNewChildren(this INode self, Type type, out INode node)
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
        public static bool TryAddNewChildren<T>(this INode self, out INode node) => self.TryAddNewChildren(typeof(T), out node);


        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public static T AddChildren<T>(this INode self)
            where T : class, INode
        {
            if (self.TryAddNewChildren<T>(out INode node))
            {
                node.SendRule<IAwakeRule>();
                self.Core.Add(node);
            }
            return node as T;
        }


        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public static T AddChildren<T, T1>(this INode self, T1 arg1)
            where T : class, INode
        {
            if (self.TryAddNewChildren<T>(out INode node))
            {
                node.SendRule<IAwakeRule<T1>, T1>(arg1);

                self.Core.Add(node);
            }
            return node as T;
        }


        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public static T AddChildren<T, T1, T2>(this INode self, T1 arg1, T2 arg2)
        where T : class, INode
        {
            if (self.TryAddNewChildren<T>(out INode node))
            {
                node.SendRule<IAwakeRule<T1, T2>, T1, T2>(arg1, arg2);

                self.Core.Add(node);
            }
            return node as T;
        }

        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public static T AddChildren<T, T1, T2, T3>(this INode self, T1 arg1, T2 arg2, T3 arg3)
        where T : class, INode
        {
            if (self.TryAddNewChildren<T>(out INode node))
            {
                node.SendRule<IAwakeRule<T1, T2, T3>, T1, T2, T3>(arg1, arg2, arg3);
                self.Core.Add(node);
            }
            return node as T;
        }

        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public static T AddChildren<T, T1, T2, T3, T4>(this INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        where T : class, INode
        {
            if (self.TryAddNewChildren<T>(out INode node))
            {
                node.SendRule<IAwakeRule<T1, T2, T3, T4>, T1, T2, T3, T4>(arg1, arg2, arg3, arg4);
                self.Core.Add(node);
            }
            return node as T;
        }

        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public static T AddChildren<T, T1, T2, T3, T4, T5>(this INode self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        where T : class, INode
        {
            if (self.TryAddNewChildren<T>(out INode node))
            {
                node.SendRule<IAwakeRule<T1, T2, T3, T4, T5>, T1, T2, T3, T4, T5>(arg1, arg2, arg3, arg4, arg5);
                self.Core.Add(node);
            }
            return node as T;
        }
        #endregion
        #endregion
    }
}
