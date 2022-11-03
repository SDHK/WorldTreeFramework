/****************************************

* 创 建 者：  闪电黑客
* 创建时间：  2022/5/6 21:27
* 描    述:   实体基类

****************************************/

using System;

namespace WorldTree
{

    /// <summary>
    /// 实体基类
    /// </summary>
    public abstract partial class Entity : IUnitPoolItem
    {
        public IPool thisPool { get; set; }

        public bool IsRecycle { get; set; }

        public bool IsDisposed { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        public long id;

        /// <summary>
        /// 实体类型
        /// </summary>
        public Type Type;

        /// <summary>
        /// 实体泛型
        /// </summary>
        public Type GenericType;

        /// <summary>
        /// 根节点
        /// </summary>
        public EntityManager Root;

        /// <summary>
        /// 父节点
        /// </summary>
        public Entity Parent;


        public Entity()
        {
            Type = GetType();
        }
     
        public override string ToString()
        {
            return GetType().ToString();
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        public T To<T>()
        where T : Entity
        {
            return this as T;
        }

        /// <summary>
        /// 父节点
        /// </summary>
        public T ParentTo<T>()
        where T : Entity
        {
            return Parent as T;
        }

        public virtual void OnDispose() { }


        /// <summary>
        /// 移除全部组件和子节点
        /// </summary>
        public void RemoveAll()
        {
            RemoveAllChildren();
            RemoveAllComponent();
        }

        /// <summary>
        /// 回收实体
        /// </summary>
        public virtual void Dispose()
        {
            if (!IsRecycle)
            {
                if (Parent != null)
                {
                    if (isComponent)
                    {
                        Parent.RemoveComponent(this);
                    }
                    else
                    {
                        Parent.RemoveChildren(this);
                    }
                }
                else 
                {
                    RemoveAll();
                }
            }
        }
    }

}
