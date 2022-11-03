
/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 11:37

* 描述： 组件节点

*/

using System;
using System.Linq;

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
        public UnitDictionary<Type, Entity> components;


        public UnitDictionary<Type, Entity> Components
        {
            get
            {
                if (components == null)
                {
                    components = Root.ObjectPoolManager.Get<UnitDictionary<Type, Entity>>();
                }
                return components;
            }

            set { components = value; }
        }


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
                component = Root.ObjectPoolManager.Get<T>();

                component.Parent = this;
                //component.Domain = Domain;

                component.isComponent = true;

                components.Add(type, component);
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
        public Entity AddComponent(Type type)
        {
            if (!Components.TryGetValue(type, out Entity component))
            {
                object obj = Root.ObjectPoolManager.Get(type);
                if (obj is Entity)
                {
                    component = obj as Entity;

                    component.Parent = this;
                    //component.Domain = Domain;

                    component.isComponent = true;

                    components.Add(type, component);
                    Root.Add(component);
                }
                else
                {
                    Root.ObjectPoolManager.Recycle(obj);
                    return null;
                }
            }
            return component;
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        public void AddComponent(Entity component)
        {
            if (component != null)
            {
                Type type = component.GetType();
                if (Components.TryAdd(type, component))
                {
                    component.Parent = this;
                    //component.Domain = Domain;
                    component.isComponent = true;
                    Root.Add(component);
                }
            }

        }

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
                Entity entity = null;
                components.TryGetValue(type, out entity);
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
                Entity entity = null;
                if (components.TryGetValue(type, out entity))
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

        /// <summary>
        /// 移除组件
        /// </summary>
        public void RemoveComponent<T>()
            where T : Entity
        {
            Type type = typeof(T);
            if (components.ContainsKey(type))
            {
                Entity component = components[type];

                Root.Remove(component);


                component.Parent = null;
                component.DisposeDomain();

                components.Remove(type);

                Root.ObjectPoolManager.Recycle(component);


                if (components.Count == 0)
                {
                    components.Dispose();
                    components = null;
                }
            }
        }
        /// <summary>
        /// 移除组件
        /// </summary>
        public void RemoveComponent(Entity component)
        {
            if (components.ContainsValue(component))
            {
                Root.Remove(component);

                component.Parent = null;
                component.DisposeDomain();

                components.Remove(component.GetType());

                Root.ObjectPoolManager.Recycle(component);

                if (components.Count == 0)
                {
                    components.Dispose();
                    components = null;
                }
            }

        }


        /// <summary>
        /// 移除全部组件
        /// </summary>
        public void RemoveAllComponent()
        {
            while (components != null)
            {
                if (components.Count != 0)
                {
                    RemoveComponent(components.Last().Value);
                }
                else
                {
                    break;
                }
            }
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
                component = Root.ObjectPoolManager.Get<T>();

                component.Parent = this;
                //component.Domain = Domain;

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
                //component.Domain = Domain;

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
                //component.Domain = Domain;

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
