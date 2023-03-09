/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/6 14:41

* 描述： 

*/

using System;

namespace WorldTree
{
    public static class NodeComponentStaticRule
    {
        #region 获取


        /// <summary>
        /// 获取组件
        /// </summary>
        public static Node GetComponent(this Node self, Type type)
        {
            if (self.m_Components == null)
            {
                return null;
            }
            else
            {
                self.m_Components.TryGetValue(type, out Node component);
                return component;
            }
        }

        /// <summary>
        /// 获取组件
        /// </summary>
        public static bool TryGetComponent(this Node self, Type type, out Node component)
        {
            if (self.m_Components == null)
            {
                component = null;
                return false;
            }
            else
            {
                return self.m_Components.TryGetValue(type, out component);
            }
        }

        /// <summary>
        /// 获取组件
        /// </summary>
        public static T GetComponent<T>(this Node self)
            where T : Node
        {
            if (self.m_Components == null)
            {
                return null;
            }
            else
            {
                Type type = typeof(T);
                self.m_Components.TryGetValue(type, out Node node);
                return node as T;
            }
        }

        /// <summary>
        /// 获取组件
        /// </summary>
        public static bool TryGetComponent<T>(this Node self, out T component)
            where T : Node
        {
            if (self.m_Components == null)
            {
                component = null;
                return false;
            }
            else
            {
                Type type = typeof(T);
                if (self.m_Components.TryGetValue(type, out Node node))
                {
                    component = node as T;
                    return true;
                }
                else
                {
                    component = null;
                    return false;
                }
            }
        }
        #endregion


        #region 移除

        /// <summary>
        /// 移除组件
        /// </summary>
        public static void RemoveComponent<T>(this Node self)
            where T : Node
        {
            Type type = typeof(T);
            self.RemoveComponent(type);
        }

        /// <summary>
        /// 移除组件
        /// </summary>
        public static void RemoveComponent(this Node self, Type type)
        {
            if (self.m_Components != null)
                if (self.m_Components.TryGetValue(type, out Node component))
                {
                    component?.Dispose();
                }
        }

        /// <summary>
        /// 移除全部组件
        /// </summary>
        public static void RemoveAllComponent(this Node self)
        {
            if (self.m_Components != null ? self.m_Components.Count != 0 : false)
            {
                var nodes = self.PoolGet<UnitStack<Node>>();
                foreach (var item in self.m_Components) nodes.Push(item.Value);

                int length = nodes.Count;
                for (int i = 0; i < length; i++)
                {
                    nodes.Pop().Dispose();
                }
                nodes.Dispose();
            }
        }

        #endregion

        #region 添加
        #region 替换或添加



        /// <summary>
        /// 添加野组件或替换父节点  （替换：从原父节点移除，移除替换同类型的组件，不调用事件）
        /// </summary>
        public static void AddComponent(this Node self, Node component)
        {
            if (component != null)
            {
                Type type = component.GetType();

                self.RemoveComponent(type);

                if (component.Parent != null)//如果父节点存在从原父节点中移除加入到当前，不调用任何事件
                {
                    component.TraversalLevelDisposeDomain();
                    component.RemoveInParent();
                    self.Components.Add(type, component);
                    component.Parent = self;
                    component.isComponent = true;
                    component.RefreshActive();
                }
                else //野组件添加
                {
                    self.Components.Add(type, component);
                    component.Parent = self;
                    component.isComponent = true;

                    component.SendRule<IAwakeRule>();
                    self.Root.Add(component);
                }
            }
        }
        #endregion
        #region 类型添加

        /// <summary>
        /// 添加组件
        /// </summary>
        public static Node AddComponent(this Node self, Type type)
        {
            if (!self.Components.TryGetValue(type, out Node component))
            {
                component = self.PoolGet(type);
                component.Parent = self;
                component.isComponent = true;
                self.m_Components.Add(type, component);
                component.SendRule<IAwakeRule>();
                self.Root.Add(component);
            }
            return component;
        }
        #endregion
        #region 泛型添加


        /// <summary>
        /// 添加组件
        /// </summary>
        public static T AddComponent<T>(this Node self, out T node) where T : Node => node = self.AddComponent<T>();
        /// <summary>
        /// 添加组件
        /// </summary>
        public static T AddComponent<T, T1>(this Node self, out T node, T1 arg1) where T : Node => node = self.AddComponent<T, T1>(arg1);
        /// <summary>
        /// 添加组件
        /// </summary>
        public static T AddComponent<T, T1, T2>(this Node self, out T node, T1 arg1, T2 arg2) where T : Node => node = self.AddComponent<T, T1, T2>(arg1, arg2);
        /// <summary>
        /// 添加组件
        /// </summary>
        public static T AddComponent<T, T1, T2, T3>(this Node self, out T node, T1 arg1, T2 arg2, T3 arg3) where T : Node => node = self.AddComponent<T, T1, T2, T3>(arg1, arg2, arg3);
        /// <summary>
        /// 添加组件
        /// </summary>
        public static T AddComponent<T, T1, T2, T3, T4>(this Node self, out T node, T1 arg1, T2 arg2, T3 arg3, T4 arg4) where T : Node => node = self.AddComponent<T, T1, T2, T3, T4>(arg1, arg2, arg3, arg4);
        /// <summary>
        /// 添加组件
        /// </summary>
        public static T AddComponent<T, T1, T2, T3, T4, T5>(this Node self, out T node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) where T : Node => node = self.AddComponent<T, T1, T2, T3, T4, T5>(arg1, arg2, arg3, arg4, arg5);


        /// <summary>
        /// 添加组件
        /// </summary>
        public static T AddComponent<T>(this Node self)
            where T : Node
        {
            Type type = typeof(T);

            T component = null;
            if (!self.Components.TryGetValue(type, out Node node))
            {
                component = self.PoolGet<T>();
                component.Parent = self;
                component.isComponent = true;
                self.m_Components.Add(type, component);
                component.SendRule<IAwakeRule>();
                self.Root.Add(component);
            }
            else
            {
                component = node as T;
            }

            return component;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        public static T AddComponent<T, T1>(this Node self, T1 arg1)
            where T : Node
        {
            Type type = typeof(T);

            T component = null;
            if (!self.Components.TryGetValue(type, out Node node))
            {
                component = self.PoolGet<T>();
                component.Parent = self;
                component.isComponent = true;
                self.m_Components.Add(type, component);
                component.SendRule<IAwakeRule<T1>, T1>(arg1);
                self.Root.Add(component);
            }
            else
            {
                component = node as T;
            }
            return component;
        }


        /// <summary>
        /// 添加组件
        /// </summary>
        public static T AddComponent<T, T1, T2>(this Node self, T1 arg1, T2 arg2)
            where T : Node
        {
            Type type = typeof(T);

            T component = null;
            if (!self.Components.TryGetValue(type, out Node node))
            {
                component = self.PoolGet<T>();
                component.Parent = self;
                component.isComponent = true;
                self.m_Components.Add(type, component);
                component.SendRule<IAwakeRule<T1, T2>, T1, T2>(arg1, arg2);
                self.Root.Add(component);
            }
            else
            {
                component = node as T;
            }
            return component;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        public static T AddComponent<T, T1, T2, T3>(this Node self, T1 arg1, T2 arg2, T3 arg3)
            where T : Node
        {
            Type type = typeof(T);

            T component = null;
            if (!self.Components.TryGetValue(type, out Node node))
            {
                component = self.PoolGet<T>();
                component.Parent = self;
                component.isComponent = true;
                self.m_Components.Add(type, component);
                component.SendRule<IAwakeRule<T1, T2, T3>, T1, T2, T3>(arg1, arg2, arg3);
                self.Root.Add(component);
            }
            else
            {
                component = node as T;
            }
            return component;
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        public static T AddComponent<T, T1, T2, T3, T4>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            where T : Node
        {
            Type type = typeof(T);

            T component = null;
            if (!self.Components.TryGetValue(type, out Node node))
            {
                component = self.PoolGet<T>();
                component.Parent = self;
                component.isComponent = true;
                self.m_Components.Add(type, component);
                component.SendRule<IAwakeRule<T1, T2, T3, T4>, T1, T2, T3, T4>(arg1, arg2, arg3, arg4);
                self.Root.Add(component);
            }
            else
            {
                component = node as T;
            }
            return component;
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        public static T AddComponent<T, T1, T2, T3, T4, T5>(this Node self, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
            where T : Node
        {
            Type type = typeof(T);

            T component = null;
            if (!self.Components.TryGetValue(type, out Node node))
            {
                component = self.PoolGet<T>();
                component.Parent = self;
                component.isComponent = true;
                self.m_Components.Add(type, component);
                component.SendRule<IAwakeRule<T1, T2, T3, T4, T5>, T1, T2, T3, T4, T5>(arg1, arg2, arg3, arg4, arg5);
                self.Root.Add(component);
            }
            else
            {
                component = node as T;
            }
            return component;
        }
        #endregion
        #endregion

    }
}
