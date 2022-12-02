
/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 11:30

* 描述： 子节点

*/

using System;
using System.Linq;

namespace WorldTree
{
    public abstract partial class Entity
    {

        /// <summary>
        /// 子节点
        /// </summary>
        public UnitDictionary<long, Entity> children;

        public UnitDictionary<long, Entity> Children
        {
            get
            {
                if (children == null)
                {
                    children = this.PoolGet<UnitDictionary<long, Entity>>();
                }
                return children;
            }
            set { children = value; }
        }


        /// <summary>
        /// 添加野节点或替换父节点 （替换：从原父节点移除，不调用事件）
        /// </summary>
        public void AddChildren(Entity entity)
        {
            if (entity != null)
            {
                if (Children.TryAdd(entity.id, entity))
                {
                    if (entity != null)
                    {
                        entity.RemoveInParent();
                        entity.Parent = this;
                        entity.isComponent = false;
                    }
                    else //野节点添加
                    {
                        entity.Parent = this;
                        entity.isComponent = false;
                        entity.SendSystem<IAwakeSystem>();
                        Root.Add(entity);
                    }
                }
            }
        }

        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public T AddChildren<T>()
            where T : Entity
        {

            T entity = Root.EntityPoolManager.Get<T>();
            if (Children.TryAdd(entity.id, entity))
            {
                entity.Parent = this;
                entity.SendSystem<IAwakeSystem>();
                Root.Add(entity);
            }

            return entity;
        }


        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public Entity AddChildren(Type type)
        {
            Entity entity = Root.PoolGet(type);
            if (Children.TryAdd(entity.id, entity))
            {
                entity.Parent = this;
                entity.SendSystem<IAwakeSystem>();
                Root.Add(entity);
            }
            return entity;
        }

        /// <summary>
        /// 通过id获取子节点
        /// </summary>
        public Entity GetChildren(long id)
        {
            if (children == null)
            {
                return null;
            }
            else
            {
                if (children.TryGetValue(id, out Entity entity))
                {
                    return entity;
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
        public bool TryGetChildren(long id, out Entity entity)
        {
            if (children == null)
            {
                entity = null;
                return false;
            }
            else
            {
                return children.TryGetValue(id, out entity);
            }
        }



        /// <summary>
        /// 移除子节点
        /// </summary>
        public void RemoveChildren(Entity entity)
        {
            entity?.Dispose();
        }
        /// <summary>
        /// 移除全部子节点
        /// </summary>
        public void RemoveAllChildren()
        {
            while (true)
            {
                if (children != null)
                    if (children.Count != 0)
                    {
                        //children.Values.GetEnumerator().Current.Dispose();
                        children.Last().Value?.Dispose();
                        continue;
                    }
                break;
            }
        }


        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public T AddChildren<T, T1>(T1 arg1)
            where T : Entity
        {
            T entity = Root.EntityPoolManager.Get<T>();
            if (Children.TryAdd(entity.id, entity))
            {
                entity.Parent = this;
                entity.SendSystem<IAwakeSystem<T1>, T1>(arg1);
                Root.Add(entity);
            }

            return entity;
        }


        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public T AddChildren<T, T1, T2>(T1 arg1, T2 arg2)
        where T : Entity
        {

            T entity = Root.EntityPoolManager.Get<T>();
            if (Children.TryAdd(entity.id, entity))
            {
                entity.Parent = this;
                entity.SendSystem<IAwakeSystem<T1, T2>, T1, T2>(arg1, arg2);
                Root.Add(entity);
            }

            return entity;
        }

        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public T AddChildren<T, T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
        where T : Entity
        {

            T entity = Root.EntityPoolManager.Get<T>();
            if (Children.TryAdd(entity.id, entity))
            {
                entity.Parent = this;
                entity.SendSystem<IAwakeSystem<T1, T2, T3>, T1, T2, T3>(arg1, arg2, arg3);
                Root.Add(entity);
            }

            return entity;
        }

        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public T AddChildren<T, T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        where T : Entity
        {
            T entity = Root.EntityPoolManager.Get<T>();
            if (Children.TryAdd(entity.id, entity))
            {
                entity.Parent = this;
                entity.SendSystem<IAwakeSystem<T1, T2, T3, T4>, T1, T2, T3, T4>(arg1, arg2, arg3, arg4);
                Root.Add(entity);
            }

            return entity;
        }

    }
}
