/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using UnityEngine;


namespace WorldTree
{

    /// <summary>
    /// GameObject对象池
    /// </summary>
    public class GameObjectPool : GenericPool<GameObject>, ChildOf<GameObjectPoolManager>
        , AsAwake
    {
        /// <summary>
        /// 预制体
        /// </summary>
        public GameObject prefab { get; set; }

        /// <summary>
        /// 游戏对象名称
        /// </summary>
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

        /// <summary>
        /// 新建对象
        /// </summary>
        private GameObject ObjectNew(IPool pool)
        {
            return (prefab == null) ? new GameObject(objName) : GameObject.Instantiate(prefab);
        }
		/// <summary>
		/// 销毁对象
		/// </summary>
		private void ObjectDestroy(GameObject gameObject)
        {
            GameObject.Destroy(gameObject);
        }
		/// <summary>
		/// 对象新建处理
		/// </summary>
		/// <param name="gameObject"></param>
		private void ObjectOnNew(GameObject gameObject)
        {

        }
		/// <summary>
		/// 对象获取处理
		/// </summary>
		private void ObjectOnGet(GameObject gameObject)
        {
            gameObject.SetActive(true);
        }
		/// <summary>
		/// 对象回收处理
		/// </summary>
		private void ObjectOnRecycle(GameObject gameObject)
        {
            gameObject.SetActive(false);
        }

    }



}