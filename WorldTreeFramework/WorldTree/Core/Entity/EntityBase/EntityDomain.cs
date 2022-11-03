
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/3 14:51

* 描述： 域节点
* 
* 用于分组，标签，获取上层节点。
* 
* 从字典查询实体是否存在，不存在则，
* 从父节点开始反向向上查询实体是否存在。
* 存在则存入字典。
* 

*/

using System;

namespace WorldTree
{
    public abstract partial class Entity
    {
        /// <summary>
        /// 域节点
        /// </summary>
        public UnitDictionary<Type, Entity> domains;

        public UnitDictionary<Type, Entity> Domains
        {
            get
            {
                if (domains == null)
                {
                    domains = Root.ObjectPoolManager.Get<UnitDictionary<Type, Entity>>();
                }
                return domains;
            }
            set { domains = value; }
        }

        /// <summary>
        /// 尝试找到上层域，并存入字典
        /// </summary>
        public bool TryFindDomain<T>(out T domain)
            where T : Entity
        {
            if (!Domains.TryGetValue(typeof(T), out Entity entity))
            {
                entity = Parent;
                while (entity != null)
                {
                    if (entity is T)
                    {
                        Domains.Add(typeof(T), entity);
                        domain = entity as T;
                        return true;
                    }
                    entity = entity.Parent;
                }
                domain = null;
                return false;
            }
            domain = entity as T;
            return true;
        }

        /// <summary>
        /// 移除域
        /// </summary>
        public void RemoveDomain<T>()
        {
            domains?.Remove(typeof(T));
        }

        /// <summary>
        /// 清除并释放域字典
        /// </summary>
        public void DisposeDomain()
        {
            domains?.Clear();
            domains?.Dispose();
            domains = null;
        }
    }
}
