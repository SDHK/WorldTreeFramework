using System;

namespace WorldTree
{
    public abstract partial class Entity

    {
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
                component = Root.ObjectPoolManager.Get<T>();

                component.Parent = this;
                component.Domain = Domain;

                component.isComponent = true;

                components.Add(type, component);
                component.SendSystem<ISendSystem<T1>, T1>(arg1);
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
                component = Root.ObjectPoolManager.Get<T>();

                component.Parent = this;
                component.Domain = Domain;

                component.isComponent = true;

                components.Add(type, component);
                component.SendSystem<ISendSystem<T1, T2>, T1, T2>(arg1, arg2);
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
                component = Root.ObjectPoolManager.Get<T>();

                component.Parent = this;
                component.Domain = Domain;

                component.isComponent = true;

                components.Add(type, component);
                component.SendSystem<ISendSystem<T1, T2, T3>, T1, T2, T3>(arg1, arg2, arg3);
                Root.Add(component);
            }
            else
            {
                component = entity as T;
            }

            return component;
        }

    }
}
