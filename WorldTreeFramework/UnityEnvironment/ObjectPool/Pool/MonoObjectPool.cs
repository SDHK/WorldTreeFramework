
/******************************

 * Author: 闪电黑客

 * 日期: 2021/12/13 13:38:13

 * 最后日期: 2021/12/15 17:49:41

 * 最后修改: 闪电黑客

 * 描述:  

    泛型Mono对象池: 继承 GenericPool<T>

    作用于继承了 MonoBehaviour、IUnitPoolItem 的类型

******************************/



using UnityEngine;

namespace WorldTree
{

    /// <summary>
    /// 泛型Mono对象池
    /// </summary>
    public class MonoObjectPool<T> : GenericPool<T>
    where T : MonoBehaviour, IUnitPoolItem
    {
        /// <summary>
        /// 池对象节点（不销毁）：用于储存回收的游戏对象
        /// </summary>
        public Transform poolTransform { get; private set; }

        /// <summary>
        /// 预制体
        /// </summary>
        public GameObject prefab { get; private set; }
       
        //游戏对象名称
        private string objName;

        /// <summary>
        /// 对象池构造 (预制体)
        /// </summary>
        public MonoObjectPool(GameObject prefabObj = null)
        {
            ObjectType = typeof(T);
            if (prefabObj != null)
            {
                prefab = prefabObj;
                objName = ObjectType.Name + "." + prefabObj.name;
            }
            else
            {
                objName = ObjectType.Name + ".MonoObject";
            }
            poolTransform = new GameObject(ToString()).transform;
            GameObject.DontDestroyOnLoad(poolTransform);

            NewObject = ObjectNew;
            DestroyObject = ObjectDestroy;

            objectOnNew += ObjectOnNew;
            objectOnDestroy += ObjectOnDestroy;
            objectOnGet += ObjectOnGet;
            objectOnRecycle += ObjectOnRecycle;


        }

        public override string ToString()
        {
            return "[MonoObjectPool<" + ObjectType + ">] : " + objName;
        }

        public override void OnDispose()
        {
            base.OnDispose();
            if (poolTransform != null)
            {
                GameObject.Destroy(poolTransform.gameObject);
            }
        }

        /// <summary>
        /// 获取对象（设置父节点）
        /// </summary>
        public T Get(Transform parent)
        {

            T obj = DequeueOrNewObject();
            obj.transform.parent = parent;
            objectOnGet?.Invoke(obj);
            Preload();

            return obj;
        }

    


        private T ObjectNew(IPool pool)
        {
            GameObject gameObj = (prefab == null) ? new GameObject(objName) : GameObject.Instantiate(prefab);
            gameObj.name = objName;
            T obj = gameObj.GetComponent<T>();
            if (obj == null)
            {
                obj = gameObj.AddComponent<T>();
            }
            obj.thisPool = pool;
            return obj;
        }

        private void ObjectDestroy(T obj)
        {
            GameObject.Destroy(obj.gameObject);
        }
        private void ObjectOnNew(T obj)
        {
            if (poolTransform == null)
            {
                GameObject.DontDestroyOnLoad(obj.gameObject);
            }
            else
            {
                obj.transform.SetParent(poolTransform);
            }

            //obj.OnNew();
        }
        private void ObjectOnGet(T obj)
        {
            obj.gameObject.SetActive(true);
            //obj.OnGet();
        }
        private void ObjectOnRecycle(T obj)
        {
            obj.gameObject.SetActive(false);
            if (poolTransform == null)
            {
                GameObject.DontDestroyOnLoad(obj.gameObject);
            }
            else
            {
                obj.transform.SetParent(poolTransform);
            }
            //obj.OnRecycle();
        }

        private void ObjectOnDestroy(T obj)
        {
            obj.Dispose();
        }

    }


}
