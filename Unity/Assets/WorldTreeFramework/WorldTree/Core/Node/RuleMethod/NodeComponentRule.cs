/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/6 14:41

* 描述： 组件节点
* 
* 用节点类型作为键值，因此同种类型的组件只能一个
* 
*/

using System;

namespace WorldTree
{
    public static class NodeComponentRule
    {
        /// <summary>
        /// 组件节点
        /// </summary>
        public static UnitSortedDictionary<long, INode> ComponentsDictionary(this INode self)
        {
            self.m_Components ??= self.PoolGet<UnitSortedDictionary<long, INode>>();
            return self.m_Components;
        }

        #region 获取

        /// <summary>
        /// 尝试获取组件
        /// </summary>
        public static bool TryGetComponent(this INode self, long type, out INode component)
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
        /// 尝试获取组件
        /// </summary>
        public static bool TryGetComponent<T>(this INode self, out T component)
            where T : class, INode
        {
            if (self.m_Components == null)
            {
                component = null;
                return false;
            }
            else
            {
                if (self.m_Components.TryGetValue(TypeInfo<T>.HashCode64, out INode node))
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
        public static void RemoveComponent<T>(this INode self)
            where T : class, INode
        {
            self.RemoveComponent(TypeInfo<T>.HashCode64);
        }

        /// <summary>
        /// 移除组件
        /// </summary>
        public static void RemoveComponent(this INode self, long type)
        {
            if (self.m_Components != null)
                if (self.m_Components.TryGetValue(type, out INode component))
                {
                    component?.Dispose();
                }
        }

        /// <summary>
        /// 移除全部组件
        /// </summary>
        public static void RemoveAllComponent(this INode self)
        {
            if (self.m_Components != null && self.m_Components.Count != 0)
            {
                var nodes = self.PoolGet<UnitStack<INode>>();
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
        public static void AddComponent<N, T>(this N self, T component)
            where N : class, INode
            where T : class, INode, ComponentOf<N>
        {
            if (component != null)
            {
                self.RemoveComponent<T>();

                if (component.Parent != null)//如果父节点存在从原父节点中移除加入到当前，不调用任何事件
                {
                    component.TraversalLevelDisposeDomain();
                    component.RemoveInParent();
                    self.ComponentsDictionary().Add(component.Type, component);
                    if (component.Branch != component) component.Branch = self.Branch;
                    component.Parent = self;
                    component.isComponent = true;
                    component.RefreshActive();
                }
                else //野组件添加
                {
                    self.ComponentsDictionary().Add(component.Type, component);
                    if (component.Branch != component) component.Branch = self.Branch;
                    component.Parent = self;
                    component.isComponent = true;
                    component.TrySendRule<IAwakeRule>();
                    self.Core.AddNode(component);
                }
            }
        }

        #endregion
        #region 类型添加

        /// <summary>
        /// 添加组件
        /// </summary>
        public static INode AddComponent(this INode self, long type)
        {
            if (!self.ComponentsDictionary().TryGetValue(type, out INode component))
            {
                component = self.PoolGet(type);
                component.Branch = self.Branch;
                component.Parent = self;
                component.isComponent = true;
                self.m_Components.Add(type, component);
                component.TrySendRule<IAwakeRule>();
                self.Core.AddNode(component);
            }
            return component;
        }
        #endregion


        /// <summary>
        /// 尝试添加新组件
        /// </summary>
        private static bool TryAddComponent(this INode self, long type, out INode component)
        {
            if (!self.ComponentsDictionary().TryGetValue(type, out component))
            {
                component = self.PoolGet(type);
                component.Branch = self.Branch;
                component.Parent = self;
                component.isComponent = true;
                self.m_Components.Add(type, component);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 尝试添加新组件
        /// </summary>
        private static bool TryAddComponent<T>(this INode self, out T Component)
            where T : class, INode
        {
            var type = TypeInfo<T>.HashCode64;
            if (!self.ComponentsDictionary().TryGetValue(type, out INode component))
            {
                component = self.PoolGet(type);
                component.Branch = self.Branch;
                component.Parent = self;
                component.isComponent = true;
                self.m_Components.Add(type, component);
                Component = component as T;
                return true;
            }
            else
            {
                Component = component as T;
                return false;
            }

        }

        /// <summary>
        /// 尝试添加新组件
        /// </summary>
        private static bool TryAddNewComponent<T>(this INode self, out T Component)
            where T : class, INode
        {
            var type = TypeInfo<T>.HashCode64;
            if (!self.ComponentsDictionary().TryGetValue(type, out INode component))
            {
                component = self.Core.NewNodeLifecycle(type);
                component.Branch = self.Branch;
                component.Parent = self;
                component.isComponent = true;
                self.m_Components.Add(type, component);
                Component = component as T;
                return true;
            }
            else
            {
                Component = component as T;
                return false;
            }

        }

        #region ComponentOf

        #region 池

        public static T AddComponent<N, T>(this N self, out T Component)
            where N : class, INode
            where T : class, INode, ComponentOf<N>, AsRule<IAwakeRule>
        {
            if (self.TryAddComponent(out Component))
            {
                Component.SendRule(DefaultType<IAwakeRule>.Default);
                self.Core.AddNode(Component);
            }
            return Component;
        }

        public static T AddComponent<N, T, T1>(this N self, out T Component, T1 arg1)
            where N : class, INode
            where T : class, INode, ComponentOf<N>, AsRule<IAwakeRule<T1>>
        {
            if (self.TryAddComponent(out Component))
            {
                Component.SendRule(DefaultType<IAwakeRule<T1>>.Default, arg1);
                self.Core.AddNode(Component);
            }
            return Component;
        }
        public static T AddComponent<N, T, T1, T2>(this N self, out T Component, T1 arg1, T2 arg2)
            where N : class, INode
            where T : class, INode, ComponentOf<N>, AsRule<IAwakeRule<T1, T2>>
        {
            if (self.TryAddComponent(out Component))
            {
                Component.SendRule(DefaultType<IAwakeRule<T1, T2>>.Default, arg1, arg2);
                self.Core.AddNode(Component);
            }
            return Component;
        }

        public static T AddComponent<N, T, T1, T2, T3>(this N self, out T Component, T1 arg1, T2 arg2, T3 arg3)
            where N : class, INode
            where T : class, INode, ComponentOf<N>, AsRule<IAwakeRule<T1, T2, T3>>
        {
            if (self.TryAddComponent(out Component))
            {
                Component.SendRule(DefaultType<IAwakeRule<T1, T2, T3>>.Default, arg1, arg2, arg3);
                self.Core.AddNode(Component);
            }
            return Component;
        }

        public static T AddComponent<N, T, T1, T2, T3, T4>(this N self, out T Component, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            where N : class, INode
            where T : class, INode, ComponentOf<N>, AsRule<IAwakeRule<T1, T2, T3, T4>>
        {
            if (self.TryAddComponent(out Component))
            {
                Component.SendRule(DefaultType<IAwakeRule<T1, T2, T3, T4>>.Default, arg1, arg2, arg3, arg4);
                self.Core.AddNode(Component);
            }
            return Component;
        }
        public static T AddComponent<N, T, T1, T2, T3, T4, T5>(this N self, out T Component, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
            where N : class, INode
            where T : class, INode, ComponentOf<N>, AsRule<IAwakeRule<T1, T2, T3, T4, T5>>
        {
            if (self.TryAddComponent(out Component))
            {
                Component.SendRule(DefaultType<IAwakeRule<T1, T2, T3, T4, T5>>.Default, arg1, arg2, arg3, arg4, arg5);
                self.Core.AddNode(Component);
            }
            return Component;
        }
        #endregion



        #region 非池

        public static T AddNewComponent<N, T>(this N self, out T Component)
           where N : class, INode
           where T : class, INode, ComponentOf<N>, AsRule<IAwakeRule>
        {
            if (self.TryAddNewComponent(out Component))
            {
                Component.SendRule(DefaultType<IAwakeRule>.Default);
                self.Core.AddNode(Component);
            }
            return Component;
        }

        public static T AddNewComponent<N, T, T1>(this N self, out T Component, T1 arg1)
         where N : class, INode
         where T : class, INode, ComponentOf<N>, AsRule<IAwakeRule<T1>>
        {
            if (self.TryAddNewComponent(out Component))
            {
                Component.SendRule(DefaultType<IAwakeRule<T1>>.Default, arg1);
                self.Core.AddNode(Component);
            }
            return Component;
        }
        public static T AddNewComponent<N, T, T1, T2>(this N self, out T Component, T1 arg1, T2 arg2)
            where N : class, INode
            where T : class, INode, ComponentOf<N>, AsRule<IAwakeRule<T1, T2>>
        {
            if (self.TryAddNewComponent(out Component))
            {
                Component.SendRule(DefaultType<IAwakeRule<T1, T2>>.Default, arg1, arg2);
                self.Core.AddNode(Component);
            }
            return Component;
        }

        public static T AddNewComponent<N, T, T1, T2, T3>(this N self, out T Component, T1 arg1, T2 arg2, T3 arg3)
            where N : class, INode
            where T : class, INode, ComponentOf<N>, AsRule<IAwakeRule<T1, T2, T3>>
        {
            if (self.TryAddNewComponent(out Component))
            {
                Component.SendRule(DefaultType<IAwakeRule<T1, T2, T3>>.Default, arg1, arg2, arg3);
                self.Core.AddNode(Component);
            }
            return Component;
        }

        public static T AddNewComponent<N, T, T1, T2, T3, T4>(this N self, out T Component, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            where N : class, INode
            where T : class, INode, ComponentOf<N>, AsRule<IAwakeRule<T1, T2, T3, T4>>
        {
            if (self.TryAddNewComponent(out Component))
            {
                Component.SendRule(DefaultType<IAwakeRule<T1, T2, T3, T4>>.Default, arg1, arg2, arg3, arg4);
                self.Core.AddNode(Component);
            }
            return Component;
        }
        public static T AddNewComponent<N, T, T1, T2, T3, T4, T5>(this N self, out T Component, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
            where N : class, INode
            where T : class, INode, ComponentOf<N>, AsRule<IAwakeRule<T1, T2, T3, T4, T5>>
        {
            if (self.TryAddNewComponent(out Component))
            {
                Component.SendRule(DefaultType<IAwakeRule<T1, T2, T3, T4, T5>>.Default, arg1, arg2, arg3, arg4, arg5);
                self.Core.AddNode(Component);
            }
            return Component;
        }

        #endregion

        #endregion

        #endregion

    }
}
