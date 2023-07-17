/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/17 14:49

* 描述： 

*/

using System;

namespace WorldTree
{

    public static class ReferencedPoolManagerRule
    {

        /// <summary>
        /// 添加节点对象
        /// </summary>
        public static bool TryAdd(this ReferencedPoolManager self, INode node)
        {
            self.allNode.TryAdd(node.Id, node);
            return self.GetPool(node.Type).TryAdd(node.Id, node);
        }

        /// <summary>
        /// 移除节点对象
        /// </summary>
        public static void Remove(this ReferencedPoolManager self, INode node)
        {
            self.allNode.Remove(node.Id);
            if (self.TryGetPool(node.Type, out ReferencedPool pool))
            {
                pool.Remove(node.Id);
            }
        }

        /// <summary>
        /// 获取池
        /// </summary>
        public static ReferencedPool GetPool(this ReferencedPoolManager self, Type type)
        {
            if (!self.pools.TryGetValue(type, out ReferencedPool pool))
            {
                pool = new ReferencedPool();
                pool.Type = pool.GetType();
                pool.Id = self.Core.IdManager.GetId();
                pool.Core = self.Core;
                pool.Root = self.Root;
                pool.Branch = self.Branch;
                pool.ReferencedType = type;
                self.pools.Add(type, pool);
                self.AddChild(pool);
            }
            return pool;
        }
        /// <summary>
        /// 尝试获取池
        /// </summary>
        public static bool TryGetPool(this ReferencedPoolManager self, Type type, out ReferencedPool pool)
        {
            return self.pools.TryGetValue(type, out pool);
        }


    }

}
