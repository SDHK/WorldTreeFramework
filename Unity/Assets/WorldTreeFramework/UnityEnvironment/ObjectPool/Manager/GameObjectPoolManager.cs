
/****************************************

* 作者： 闪电黑客
* 日期： 2022/10/10 16:34

* 描述： 游戏对象池管理器

*/

using UnityEngine;

namespace WorldTree
{

    public static class GameObjectPoolManagerExtension
    {
        /// <summary>
        /// 获取游戏对象池管理器
        /// </summary>
        public static GameObjectPoolManager GamePoolManager(this INode self)
        {
            return self.Root.AddComponent(out GameObjectPoolManager _);
        }

        /// <summary>
        /// 通过预制体从池中获取游戏对象
        /// </summary>
        public static GameObject PoolGet(this INode self, GameObject prefab)
        {
            return self.Root.AddComponent(out GameObjectPoolManager _).Get(prefab);
        }

        /// <summary>
        /// 通过预制体回收游戏对象
        /// </summary>
        public static void PoolRecycle(this INode self, GameObject prefab, GameObject obj)
        {
            self.Root.AddComponent(out GameObjectPoolManager _).Recycle(prefab, obj);
        }
    }


    /// <summary>
    /// 游戏对象池管理器
    /// </summary>
    public class GameObjectPoolManager : Node, ComponentOf<WorldTreeRoot>
        
    {
        UnitDictionary<GameObject, GameObjectPool> pools = new UnitDictionary<GameObject, GameObjectPool>();

        /// <summary>
        /// 获取实例
        /// </summary>
        public GameObject Get(GameObject prefab)
        {
            return GetPool(prefab).Get();
        }

        /// <summary>
        /// 回收实例
        /// </summary>
        public void Recycle(GameObject prefab, GameObject gameObject)
        {
            GetPool(prefab).Recycle(gameObject);
        }

        /// <summary>
        /// 获取池
        /// </summary>
        public GameObjectPool GetPool(GameObject prefab)
        {
            if (!pools.TryGetValue(prefab, out GameObjectPool pool))
            {
                this.AddChild(out pool);
                pool.SetPrefab(prefab);
                pools.Add(prefab, pool);
            }
            return pool;
        }

        /// <summary>
        /// 释放池
        /// </summary>
        public void DisposePool(GameObject prefab)
        {
            if (pools.TryGetValue(prefab, out GameObjectPool pool))
            {
                pool.Dispose();
                pools.Remove(prefab);
            }
        }
    }
}
