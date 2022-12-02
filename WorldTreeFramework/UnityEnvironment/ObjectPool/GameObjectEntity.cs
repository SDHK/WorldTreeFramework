
/****************************************

* 作者： 闪电黑客
* 日期： 2022/10/11 10:06

* 描述： 游戏物体实体
* 
* 将GameObject包装为实体
* 可随着父节点一起回收到对象池

*/

using UnityEngine;

namespace WorldTree
{
    /// <summary>
    /// 游戏物体实体
    /// </summary>
    public class GameObjectEntity : Entity
    {
        public GameObjectPool pool;

        public GameObject gameObject;
        public Transform transform;

        /// <summary>
        /// 尝试实例化预制体
        /// </summary>
        public bool TryInstantiate(GameObject prefab,out GameObject obj)
        {
            if (prefab)
            {
                if (gameObject != null) pool?.Recycle(gameObject);

                pool = this.GamePoolManager().GetPool(prefab);
                gameObject = pool.Get();
                transform = gameObject.transform;

                obj = gameObject;
                return true;
            }
            else
            {
                obj = null;
                return false;
            }
        }

        /// <summary>
        /// 实例化
        /// </summary>
        public GameObjectEntity Instantiate(GameObject prefab)
        {
            if (prefab)
            {
                if (gameObject != null) pool?.Recycle(gameObject);
                pool = this.GamePoolManager().GetPool(prefab);
                gameObject = pool.Get();
                transform = gameObject.transform;
            }
            return this;
        }
        
        /// <summary>
        /// 实例化
        /// </summary>
        public GameObjectEntity Instantiate(GameObject prefab, Transform parent)
        {
            if (TryInstantiate(prefab,out GameObject gameObject))
            {
                gameObject.transform.SetParent(parent);
            }
            return this;
        }

        /// <summary>
        /// 实例化
        /// </summary>
        public GameObjectEntity Instantiate(GameObject prefab, GameObjectEntity parent)
        {
            if (TryInstantiate(prefab, out GameObject gameObject))
            {
                gameObject.transform.SetParent(parent.transform);
            }
            return this;
        }
    }

    class GameObjectEntityRemoveSystem : RemoveSystem<GameObjectEntity>
    {
        public override void OnRemove(GameObjectEntity self)
        {
            self.pool?.Recycle(self.gameObject);
            self.gameObject = null;
            self.transform = null;
            self.pool = null;
        }
    }
}
