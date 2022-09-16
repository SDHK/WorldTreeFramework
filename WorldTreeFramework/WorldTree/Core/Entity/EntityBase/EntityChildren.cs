namespace WorldTree
{
    public abstract partial class Entity
    {
        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public T AddChildren<T, T1>(T1 arg1)
            where T : Entity
        {

            T entity = Root.ObjectPoolManager.Get<T>();
            if (Children.TryAdd(entity.id, entity))
            {
                entity.Parent = this;
                entity.Domain = Domain;
                entity.SendSystem<ISendSystem<T1>, T1>(arg1);
                Root.Add(entity);
            }

            return entity;
        }


        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public T AddChildren<T, T1,T2>(T1 arg1,T2 arg2)
        where T : Entity
        {

            T entity = Root.ObjectPoolManager.Get<T>();
            if (Children.TryAdd(entity.id, entity))
            {
                entity.Parent = this;
                entity.Domain = Domain;
                entity.SendSystem<ISendSystem<T1, T2>, T1, T2>(arg1, arg2);
                Root.Add(entity);
            }

            return entity;
        }

        /// <summary>
        /// 添加新的子节点
        /// </summary>
        public T AddChildren<T, T1, T2,T3>(T1 arg1, T2 arg2, T3 arg3)
        where T : Entity
        {

            T entity = Root.ObjectPoolManager.Get<T>();
            if (Children.TryAdd(entity.id, entity))
            {
                entity.Parent = this;
                entity.Domain = Domain;
                entity.SendSystem<ISendSystem<T1, T2, T3>, T1, T2, T3>(arg1, arg2,arg3);
                Root.Add(entity);
            }

            return entity;
        }

    }
}
