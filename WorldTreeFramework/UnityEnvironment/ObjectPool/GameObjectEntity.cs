
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

        public override string ToString()
        {
            return gameObject? "GameObjectEntity : " + gameObject.name: "GameObjectEntity : ";
        }

        /// <summary>
        /// 回收或删除游戏物体
        /// </summary>
        public void DestroyGameObject()
        {
            if (gameObject != null)
            {
                if (pool != null)
                {
                    pool.Recycle(gameObject);
                }
                else
                {
                    GameObject.Destroy(gameObject);
                }
            }
        }

        /// <summary>
        /// 用类名实例化一个空物体
        /// </summary>
        public GameObjectEntity Instantiate(string name)
        {
            DestroyGameObject();
            gameObject = new GameObject(name);
            transform = gameObject.transform;
            transform.Default();
            return this;
        }

        /// <summary>
        /// 用类名实例化一个空物体
        /// </summary>
        public GameObjectEntity Instantiate(string name, Transform parent)
        {
            Instantiate(name);
            transform.SetParent(parent);
            return this;
        }
        /// <summary>
        /// 用类名实例化一个空物体
        /// </summary>
        public GameObjectEntity Instantiate(string name, GameObjectEntity parent)
        {
            Instantiate(name);
            transform.SetParent(parent.transform);
            return this;
        }


        /// <summary>
        /// 用类名实例化一个空物体
        /// </summary>
        public GameObjectEntity Instantiate<T>() where T : class => Instantiate(typeof(T).Name);

        /// <summary>
        /// 用类名实例化一个空物体
        /// </summary>
        public GameObjectEntity Instantiate<T>(Transform parent) where T : class => Instantiate(typeof(T).Name, parent);

        /// <summary>
        /// 用类名实例化一个空物体
        /// </summary>
        public GameObjectEntity Instantiate<T>(GameObjectEntity parent) where T : class => Instantiate(typeof(T).Name, parent);


        /// <summary>
        /// 尝试实例化预制体
        /// </summary>
        public bool TryInstantiate(GameObject prefab, out GameObject obj)
        {
            if (prefab)
            {
                DestroyGameObject();
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
                DestroyGameObject();
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
            if (TryInstantiate(prefab, out GameObject gameObject))
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
        public override void OnEvent(GameObjectEntity self)
        {
            self.DestroyGameObject();
            self.gameObject = null;
            self.transform = null;
            self.pool = null;
        }
    }
}
