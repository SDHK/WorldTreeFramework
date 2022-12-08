
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
        /// 域节点:归0时回收，并设为null
        /// </summary>
        public UnitDictionary<Type, Entity> domains;

        /// <summary>
        /// 域节点:为null时从池里获取
        /// </summary>
        public UnitDictionary<Type, Entity> Domains
        {
            get
            {
                if (domains == null)
                {
                    domains = this.PoolGet<UnitDictionary<Type, Entity>>();
                }
                return domains;
            }
            set { domains = value; }
        }

        /// <summary>
        /// 获取所有上层节点并存入字典
        /// </summary>
        public T GetDomain<T>()
            where T : Entity
        {
            TryGetDomain(out T domain);
            return domain;
        }

        /// <summary>
        /// 获取所有上层节点并存入字典
        /// </summary>
        public bool TryGetDomain<T>(out T domain)
            where T : Entity
        {

            if (Domains.TryGetValue(typeof(T), out Entity entity))
            {
                domain = entity as T;
                return true;
            }
            else if (Domains.Count == 0)
            {
                entity = Parent;
                while (entity != null)
                {
                    Domains.TryAdd(entity.GetType(), entity);
                    entity = entity.Parent;
                }
                if (Domains.TryGetValue(typeof(T), out entity))
                {
                    domain = entity as T;
                    return true;
                }
            }

            domain = null;
            return false;
        }

    }
}
