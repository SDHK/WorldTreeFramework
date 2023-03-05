
/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 11:30

* 描述： 子节点
* 
* 用节点ID作为键值，因此可以添加相同类型的子节点

*/

using System;


namespace WorldTree
{
    public abstract partial class Node
    {
        /// <summary>
        /// 子节点
        /// </summary>
        private UnitDictionary<long, Node> children;

        /// <summary>
        /// 子节点
        /// </summary>
        public UnitDictionary<long, Node> Children
        {
            get
            {
                if (children == null)
                {
                    children = this.PoolGet<UnitDictionary<long, Node>>();
                }
                return children;
            }
            set { children = value; }
        }

        #region 获取

        /// <summary>
        /// 通过id获取子节点
        /// </summary>
        public Node GetChildren(long id)
        {
            if (children == null)
            {
                return null;
            }
            else
            {
                if (children.TryGetValue(id, out Node node))
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
        public bool TryGetChildren(long id, out Node node)
        {
            if (children == null)
            {
                node = null;
                return false;
            }
            else
            {
                return children.TryGetValue(id, out node);
            }
        }


        #endregion

        #region 移除
        /// <summary>
        /// 移除子节点
        /// </summary>
        public void RemoveChildren(Node node)
        {
            node?.Dispose();
        }

        /// <summary>
        /// 移除全部子节点
        /// </summary>
        public void RemoveAllChildren()
        {
            if (children != null ? children.Count != 0 : false)
            {
                var entitys = Root.PoolGet<UnitStack<Node>>();
                foreach (var item in children) entitys.Push(item.Value);

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
        public void AddChildren(Node node)
        {
            if (node != null)
            {
                if (Children.TryAdd(node.id, node))
                {
                    if (node.Parent != null)
                    {
                        node.TraversalLevelDisposeDomain();
                        node.RemoveInParent();
                        node.Parent = this;
                        node.isComponent = false;
                        node.RefreshActive();
                    }
                    else //野节点添加
                    {
                        node.Parent = this;
                        node.isComponent = false;
                        node.SendSystem<IAwakeSystem>();
                        Root.Add(node);
                    }
                }
            }
        }
        #endregion

        #region 类型添加

        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public Node AddChildren(Type type)
        {
            Node node = Root.PoolGet(type);
            if (Children.TryAdd(node.id, node))
            {
                node.Parent = this;
                node.SendSystem<IAwakeSystem>();
                Root.Add(node);
            }
            return node;
        }
        #endregion

        #region 泛型添加

        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public T AddChildren<T>(out T node) where T : Node => node = AddChildren<T>();
        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public T AddChildren<T, T1>(out T node, T1 arg1) where T : Node => node = AddChildren<T, T1>(arg1);
        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public T AddChildren<T, T1, T2>(out T node, T1 arg1, T2 arg2) where T : Node => node = AddChildren<T, T1, T2>(arg1, arg2);
        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public T AddChildren<T, T1, T2, T3>(out T node, T1 arg1, T2 arg2, T3 arg3) where T : Node => node = AddChildren<T, T1, T2, T3>(arg1, arg2, arg3);
        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public T AddChildren<T, T1, T2, T3, T4>(out T node, T1 arg1, T2 arg2, T3 arg3, T4 arg4) where T : Node => node = AddChildren<T, T1, T2, T3, T4>(arg1, arg2, arg3, arg4);
        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public T AddChildren<T, T1, T2, T3, T4, T5>(out T node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) where T : Node => node = AddChildren<T, T1, T2, T3, T4, T5>(arg1, arg2, arg3, arg4, arg5);
      
        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public T AddChildren<T>()
            where T : Node
        {

            T node = Root.EntityPoolManager.Get<T>();
            if (Children.TryAdd(node.id, node))
            {
                node.Parent = this;
                node.SendSystem<IAwakeSystem>();
                Root.Add(node);
            }

            return node;
        }


        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public T AddChildren<T, T1>(T1 arg1)
            where T : Node
        {
            T node = Root.EntityPoolManager.Get<T>();
            if (Children.TryAdd(node.id, node))
            {
                node.Parent = this;
                node.SendSystem<IAwakeSystem<T1>, T1>(arg1);
                Root.Add(node);
            }
            return node;
        }


        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public T AddChildren<T, T1, T2>(T1 arg1, T2 arg2)
        where T : Node
        {
            T node = Root.EntityPoolManager.Get<T>();
            if (Children.TryAdd(node.id, node))
            {
                node.Parent = this;
                node.SendSystem<IAwakeSystem<T1, T2>, T1, T2>(arg1, arg2);
                Root.Add(node);
            }
            return node;
        }

        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public T AddChildren<T, T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
        where T : Node
        {
            T node = Root.EntityPoolManager.Get<T>();
            if (Children.TryAdd(node.id, node))
            {
                node.Parent = this;
                node.SendSystem<IAwakeSystem<T1, T2, T3>, T1, T2, T3>(arg1, arg2, arg3);
                Root.Add(node);
            }
            return node;
        }

        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public T AddChildren<T, T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        where T : Node
        {
            T node = Root.EntityPoolManager.Get<T>();
            if (Children.TryAdd(node.id, node))
            {
                node.Parent = this;
                node.SendSystem<IAwakeSystem<T1, T2, T3, T4>, T1, T2, T3, T4>(arg1, arg2, arg3, arg4);
                Root.Add(node);
            }

            return node;
        }

        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public T AddChildren<T, T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        where T : Node
        {
            T node = Root.EntityPoolManager.Get<T>();
            if (Children.TryAdd(node.id, node))
            {
                node.Parent = this;
                node.SendSystem<IAwakeSystem<T1, T2, T3, T4, T5>, T1, T2, T3, T4, T5>(arg1, arg2, arg3, arg4, arg5);
                Root.Add(node);
            }

            return node;
        }
        #endregion
        #endregion

    }
}
