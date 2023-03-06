/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/6 14:29

* 描述： 

*/

using System;

namespace WorldTree
{
    public static class NodeChildrenStaticRule
    {

        #region 获取
        /// <summary>
        /// 通过id获取子节点
        /// </summary>
        public static Node GetChildren(this Node self, long id)
        {
            if (self.m_Children == null)
            {
                return null;
            }
            else
            {
                if (self.m_Children.TryGetValue(id, out Node node))
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
        public static bool TryGetChildren(this Node self, long id, out Node node)
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
        public static void RemoveChildren(this Node self, long id)
        {
            if (self.m_Children != null)
            {
                if (self.m_Children.TryGetValue(id, out Node node))
                {
                    node.Dispose();
                }
            }
        }

        /// <summary>
        /// 移除全部子节点
        /// </summary>
        public static void RemoveAllChildren(this Node self)
        {
            if (self.m_Children != null ? self.m_Children.Count != 0 : false)
            {
                var entitys = self.Root.PoolGet<UnitStack<Node>>();
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
        public static void AddChildren(this Node self, Node node)
        {
            if (node != null)
            {
                if (self.Children.TryAdd(node.id, node))
                {
                    if (node.Parent != null)
                    {
                        node.TraversalLevelDisposeDomain();
                        node.RemoveInParent();
                        node.Parent = self;
                        node.isComponent = false;
                        node.RefreshActive();
                    }
                    else //野节点添加
                    {
                        node.Parent = self;
                        node.isComponent = false;
                        node.SendRule<IAwakeRule>();
                        self.Root.Add(node);
                    }
                }
            }
        }
        #endregion

        #region 类型添加

        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public static Node AddChildren(this Node self, Type type)
        {
            Node node = self.Root.PoolGet(type);
            if (self.Children.TryAdd(node.id, node))
            {
                node.Parent = self;
                node.SendRule<IAwakeRule>();
                self.Root.Add(node);
            }
            return node;
        }
        #endregion

        #region 泛型添加

        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public static T AddChildren<T>(this Node self, out T node) where T : Node => node = self.AddChildren<T>();
        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public static T AddChildren<T, T1>(this Node self, out T node, T1 arg1) where T : Node => node = self.AddChildren<T, T1>(arg1);
        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public static T AddChildren<T, T1, T2>(this Node self, out T node, T1 arg1, T2 arg2) where T : Node => node = self.AddChildren<T, T1, T2>(arg1, arg2);
        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public static T AddChildren<T, T1, T2, T3>(this Node self, out T node, T1 arg1, T2 arg2, T3 arg3) where T : Node => node = self.AddChildren<T, T1, T2, T3>(arg1, arg2, arg3);
        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public static T AddChildren<T, T1, T2, T3, T4>(this Node self, out T node, T1 arg1, T2 arg2, T3 arg3, T4 arg4) where T : Node => node = self.AddChildren<T, T1, T2, T3, T4>(arg1, arg2, arg3, arg4);
        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public static T AddChildren<T, T1, T2, T3, T4, T5>(this Node self, out T node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) where T : Node => node = self.AddChildren<T, T1, T2, T3, T4, T5>(arg1, arg2, arg3, arg4, arg5);

        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public static T AddChildren<T>(this Node self)
            where T : Node
        {

            T node = self.Root.NodePoolManager.Get<T>();
            if (self.Children.TryAdd(node.id, node))
            {
                node.Parent = self;
                node.SendRule<IAwakeRule>();
                self.Root.Add(node);
            }

            return node;
        }


        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public static T AddChildren<T, T1>(this Node self, T1 arg1)
            where T : Node
        {
            T node = self.Root.NodePoolManager.Get<T>();
            if (self.Children.TryAdd(node.id, node))
            {
                node.Parent = self;
                node.SendRule<IAwakeRule<T1>, T1>(arg1);
                self.Root.Add(node);
            }
            return node;
        }


        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public static T AddChildren<T, T1, T2>(this Node self, T1 arg1, T2 arg2)
        where T : Node
        {
            T node = self.Root.NodePoolManager.Get<T>();
            if (self.Children.TryAdd(node.id, node))
            {
                node.Parent = self;
                node.SendRule<IAwakeRule<T1, T2>, T1, T2>(arg1, arg2);
                self.Root.Add(node);
            }
            return node;
        }

        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public static T AddChildren<T, T1, T2, T3>(this Node self, T1 arg1, T2 arg2, T3 arg3)
        where T : Node
        {
            T node = self.Root.NodePoolManager.Get<T>();
            if (self.Children.TryAdd(node.id, node))
            {
                node.Parent = self;
                node.SendRule<IAwakeRule<T1, T2, T3>, T1, T2, T3>(arg1, arg2, arg3);
                self.Root.Add(node);
            }
            return node;
        }

        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public static T AddChildren<T, T1, T2, T3, T4>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        where T : Node
        {
            T node = self.Root.NodePoolManager.Get<T>();
            if (self.Children.TryAdd(node.id, node))
            {
                node.Parent = self;
                node.SendRule<IAwakeRule<T1, T2, T3, T4>, T1, T2, T3, T4>(arg1, arg2, arg3, arg4);
                self.Root.Add(node);
            }

            return node;
        }

        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public static T AddChildren<T, T1, T2, T3, T4, T5>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        where T : Node
        {
            T node = self.Root.NodePoolManager.Get<T>();
            if (self.Children.TryAdd(node.id, node))
            {
                node.Parent = self;
                node.SendRule<IAwakeRule<T1, T2, T3, T4, T5>, T1, T2, T3, T4, T5>(arg1, arg2, arg3, arg4, arg5);
                self.Root.Add(node);
            }

            return node;
        }
        #endregion
        #endregion
    }
}
