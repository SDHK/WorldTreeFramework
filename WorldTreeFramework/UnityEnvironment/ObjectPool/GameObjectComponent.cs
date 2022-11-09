
/****************************************

* 作者： 闪电黑客
* 日期： 2022/10/11 10:06

* 描述： 游戏物体组件
* 
* 将GameObject包装为组件
* 可随着父节点一起回收

*/

using UnityEngine;

namespace WorldTree
{
    /// <summary>
    /// 游戏物体组件
    /// </summary>
    public class GameObjectComponent : Entity
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
        public GameObjectComponent Instantiate(GameObject prefab)
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
        public GameObjectComponent Instantiate(GameObject prefab, Transform parent)
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
        public GameObjectComponent Instantiate(GameObject prefab, GameObjectComponent parent)
        {
            if (TryInstantiate(prefab, out GameObject gameObject))
            {
                gameObject.transform.SetParent(parent.transform);
            }
            return this;
        }
    }

    class GameObjectComponentRemoveSystem : RemoveSystem<GameObjectComponent>
    {
        public override void OnRemove(GameObjectComponent self)
        {
            self.pool?.Recycle(self.gameObject);
            self.gameObject = null;
            self.transform = null;
            self.pool = null;
        }
    }
}
