
/******************************

 * Author: 闪电黑客

 * 日期: 2021/12/13 04:33:09

 * 最后日期: 2021/12/15 17:46:40

 * 最后修改: 闪电黑客

 * 描述:  

    游戏物体对象池: 继承 GenericPool<LR>

    作用于 GameObject 类型

******************************/



using UnityEngine;


namespace WorldTree
{

    /// <summary>
    /// GameObject对象池
    /// </summary>
    public class GameObjectPool : GenericPool<GameObject>, ChildOf<GameObjectPoolManager>
    {
        /// <summary>
        /// 预制体
        /// </summary>
        public GameObject prefab { get; set; }

        //游戏对象名称
        private string objName;

        public GameObjectPool()
        {
            ObjectType = typeof(GameObject);

            objName = "GameObject";

            NewObject = ObjectNew;
            DestroyObject = ObjectDestroy;

            objectOnNew += ObjectOnNew;
            objectOnGet += ObjectOnGet;
            objectOnRecycle += ObjectOnRecycle;
        }

        /// <summary>
        /// 设置预制体
        /// </summary>
        public void SetPrefab(GameObject prefab)
        {
            if (prefab != null)
            {
                this.prefab = prefab;
                objName = prefab.name;
            }
        }
        public override string ToString()
        {
            return "[GameObjectPool] : " + objName;
        }

        public override void OnDispose()
        {
            base.OnDispose();
        }

        /// <summary>
        /// 获取对象（设置父节点）
        /// </summary>
        public GameObject Get(Transform parent)
        {
            GameObject gameObject = DequeueOrNewObject();
            gameObject.transform.parent = parent;
            objectOnGet?.Invoke(gameObject);
            Preload();

            return gameObject;
        }

        private GameObject ObjectNew(IPool pool)
        {
            return (prefab == null) ? new GameObject(objName) : GameObject.Instantiate(prefab);
        }
        private void ObjectDestroy(GameObject gameObject)
        {
            GameObject.Destroy(gameObject);
        }

        private void ObjectOnNew(GameObject gameObject)
        {

        }
        private void ObjectOnGet(GameObject gameObject)
        {
            gameObject.SetActive(true);
        }
        private void ObjectOnRecycle(GameObject gameObject)
        {
            gameObject.SetActive(false);
        }

    }



}