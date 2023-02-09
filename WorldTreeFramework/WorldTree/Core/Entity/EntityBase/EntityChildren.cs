
/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 11:30

* 描述： 子节点
* 
* 用实体ID作为键值，因此可以添加相同类型的子节点

*/

using System;


namespace WorldTree
{
    public abstract partial class Entity
    {
        /// <summary>
        /// 子节点
        /// </summary>
        private UnitDictionary<long, Entity> children;

        /// <summary>
        /// 子节点
        /// </summary>
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

        #region 获取

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


        #endregion

        #region 移除
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
            if (children != null ? children.Count != 0 : false)
            {
                var entitys = Root.PoolGet<UnitStack<Entity>>();
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
        public void AddChildren(Entity entity)
        {
            if (entity != null)
            {
                if (Children.TryAdd(entity.id, entity))
                {
                    if (entity.Parent != null)
                    {
                        entity.TraversalLevelDisposeDomain();
                        entity.RemoveInParent();
                        entity.Parent = this;
                        entity.isComponent = false;
                        entity.RefreshActive();
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
        #endregion

        #region 类型添加

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
        #endregion

        #region 泛型添加

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

        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public T AddChildren<T, T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        where T : Entity
        {
            T entity = Root.EntityPoolManager.Get<T>();
            if (Children.TryAdd(entity.id, entity))
            {
                entity.Parent = this;
                entity.SendSystem<IAwakeSystem<T1, T2, T3, T4, T5>, T1, T2, T3, T4, T5>(arg1, arg2, arg3, arg4, arg5);
                Root.Add(entity);
            }

            return entity;
        }
        #endregion
        #endregion

    }
}
