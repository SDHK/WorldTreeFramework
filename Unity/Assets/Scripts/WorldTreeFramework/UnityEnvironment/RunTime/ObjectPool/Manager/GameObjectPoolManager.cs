/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using UnityEngine;

namespace WorldTree
{
    /// <summary>
    /// 游戏对象池管理器
    /// </summary>
    public class GameObjectPoolManager : Node, ComponentOf<World>
        , AsChildBranch
        , AsAwake
    {
        UnitDictionary<GameObject, GameObjectPool> poolDict = new UnitDictionary<GameObject, GameObjectPool>();

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
            if (!poolDict.TryGetValue(prefab, out GameObjectPool pool))
            {
                this.AddChild(out pool);
                pool.SetPrefab(prefab);
                poolDict.Add(prefab, pool);
            }
            return pool;
        }

        /// <summary>
        /// 释放池
        /// </summary>
        public void DisposePool(GameObject prefab)
        {
            if (poolDict.TryGetValue(prefab, out GameObjectPool pool))
            {
                pool.Dispose();
                poolDict.Remove(prefab);
            }
        }
    }
}
