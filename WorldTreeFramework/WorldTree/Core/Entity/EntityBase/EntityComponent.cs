
/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 11:37

* 描述： 组件节点
* 
* 用实体类型作为键值，因此同种类型的组件只能一个

*/

using System;

namespace WorldTree
{
    public abstract partial class Entity

    {
        /// <summary>
        /// 组件标记
        /// </summary>
        public bool isComponent;

        /// <summary>
        /// 组件节点
        /// </summary>
        private UnitDictionary<Type, Entity> components;

        /// <summary>
        /// 组件节点
        /// </summary>
        public UnitDictionary<Type, Entity> Components
        {
            get
            {
                if (components == null)
                {
                    components = Root.PoolGet<UnitDictionary<Type, Entity>>();
                }
                return components;
            }

            set { components = value; }
        }

        #region 获取


        /// <summary>
        /// 获取组件
        /// </summary>
        public Entity GetComponent(Type type)
        {
            if (components == null)
            {
                return null;
            }
            else
            {
                components.TryGetValue(type, out Entity component);
                return component;
            }
        }

        /// <summary>
        /// 获取组件
        /// </summary>
        public bool TryGetComponent(Type type, out Entity component)
        {
            if (components == null)
            {
                component = null;
                return false;
            }
            else
            {
                return components.TryGetValue(type, out component);
            }
        }

        /// <summary>
        /// 获取组件
        /// </summary>
        public T GetComponent<T>()
            where T : Entity
        {
            if (components == null)
            {
                return null;
            }
            else
            {
                Type type = typeof(T);
                components.TryGetValue(type, out Entity entity);
                return entity as T;
            }
        }

        /// <summary>
        /// 获取组件
        /// </summary>
        public bool TryGetComponent<T>(out T component)
            where T : Entity
        {
            if (components == null)
            {
                component = null;
                return false;
            }
            else
            {
                Type type = typeof(T);
                if (components.TryGetValue(type, out Entity entity))
                {
                    component = entity as T;
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
        public void RemoveComponent<T>()
            where T : Entity
        {
            Type type = typeof(T);
            RemoveComponent(type);
        }

        /// <summary>
        /// 移除组件
        /// </summary>
        public void RemoveComponent(Type type)
        {
            if (components != null)
                if (components.TryGetValue(type, out Entity component))
                {
                    component?.Dispose();
                }
        }

        /// <summary>
        /// 移除组件
        /// </summary>
        public void RemoveComponent(Entity component)
        {
            component?.Dispose();
        }


        /// <summary>
        /// 移除全部组件
        /// </summary>
        public void RemoveAllComponent()
        {
            if (components != null ? components.Count != 0 : false)
            {
                var entitys = Root.PoolGet<UnitStack<Entity>>();
                foreach (var item in components) entitys.Push(item.Value);

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
        /// 添加野组件或替换父节点  （替换：从原父节点移除，移除替换同类型的组件，不调用事件）
        /// </summary>
        public void AddComponent(Entity component)
        {
            if (component != null)
            {
                Type type = component.GetType();

                RemoveComponent(type);

                if (component.Parent != null)//如果父节点存在从原父节点中移除加入到当前，不调用任何事件
                {
                    component.TraversalLevelDisposeDomain();
                    component.RemoveInParent();
                    Components.Add(type, component);
                    component.Parent = this;
                    component.isComponent = true;
                    component.RefreshActive();
                }
                else //野组件添加
                {
                    Components.Add(type, component);
                    component.Parent = this;
                    component.isComponent = true;

                    component.SendSystem<IAwakeSystem>();
                    Root.Add(component);
                }
            }
        }
        #endregion
        #region 类型添加

        /// <summary>
        /// 添加组件
        /// </summary>
        public Entity AddComponent(Type type)
        {
            if (!Components.TryGetValue(type, out Entity component))
            {
                component = Root.PoolGet(type);
                component.Parent = this;
                component.isComponent = true;
                components.Add(type, component);
                component.SendSystem<IAwakeSystem>();
                Root.Add(component);
            }
            return component;
        }
        #endregion
        #region 泛型添加


        /// <summary>
        /// 添加组件
        /// </summary>
        public T AddComponent<T>()
            where T : Entity
        {
            Type type = typeof(T);

            T component = null;
            if (!Components.TryGetValue(type, out Entity entity))
            {
                component = Root.EntityPoolManager.Get<T>();
                component.Parent = this;
                component.isComponent = true;
                components.Add(type, component);
                component.SendSystem<IAwakeSystem>();
                Root.Add(component);
            }
            else
            {
                component = entity as T;
            }

            return component;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        public T AddComponent<T, T1>(T1 arg1)
            where T : Entity
        {
            Type type = typeof(T);

            T component = null;
            if (!Components.TryGetValue(type, out Entity entity))
            {
                component = Root.EntityPoolManager.Get<T>();
                component.Parent = this;
                component.isComponent = true;
                components.Add(type, component);
                component.SendSystem<IAwakeSystem<T1>, T1>(arg1);
                Root.Add(component);
            }
            else
            {
                component = entity as T;
            }
            return component;
        }


        /// <summary>
        /// 添加组件
        /// </summary>
        public T AddComponent<T, T1, T2>(T1 arg1, T2 arg2)
            where T : Entity
        {
            Type type = typeof(T);

            T component = null;
            if (!Components.TryGetValue(type, out Entity entity))
            {
                component = Root.EntityPoolManager.Get<T>();
                component.Parent = this;
                component.isComponent = true;
                components.Add(type, component);
                component.SendSystem<IAwakeSystem<T1, T2>, T1, T2>(arg1, arg2);
                Root.Add(component);
            }
            else
            {
                component = entity as T;
            }
            return component;
        }
        /// <summary>
        /// 添加组件
        /// </summary>
        public T AddComponent<T, T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
            where T : Entity
        {
            Type type = typeof(T);

            T component = null;
            if (!Components.TryGetValue(type, out Entity entity))
            {
                component = Root.EntityPoolManager.Get<T>();
                component.Parent = this;
                component.isComponent = true;
                components.Add(type, component);
                component.SendSystem<IAwakeSystem<T1, T2, T3>, T1, T2, T3>(arg1, arg2, arg3);
                Root.Add(component);
            }
            else
            {
                component = entity as T;
            }
            return component;
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        public T AddComponent<T, T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            where T : Entity
        {
            Type type = typeof(T);

            T component = null;
            if (!Components.TryGetValue(type, out Entity entity))
            {
                component = Root.EntityPoolManager.Get<T>();
                component.Parent = this;
                component.isComponent = true;
                components.Add(type, component);
                component.SendSystem<IAwakeSystem<T1, T2, T3, T4>, T1, T2, T3, T4>(arg1, arg2, arg3, arg4);
                Root.Add(component);
            }
            else
            {
                component = entity as T;
            }
            return component;
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        public T AddComponent<T, T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
            where T : Entity
        {
            Type type = typeof(T);

            T component = null;
            if (!Components.TryGetValue(type, out Entity entity))
            {
                component = Root.EntityPoolManager.Get<T>();
                component.Parent = this;
                component.isComponent = true;
                components.Add(type, component);
                component.SendSystem<IAwakeSystem<T1, T2, T3, T4, T5>, T1, T2, T3, T4, T5>(arg1, arg2, arg3, arg4, arg5);
                Root.Add(component);
            }
            else
            {
                component = entity as T;
            }
            return component;
        }
        #endregion
        #endregion
    }
}
