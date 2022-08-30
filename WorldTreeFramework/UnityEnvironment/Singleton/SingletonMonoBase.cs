/****************************************

* 创 建 者：  闪电黑客
* 创建时间：  2022/5/6 22:48
* 描    述:

****************************************/
using UnityEngine;

namespace WorldTree
{
    /// <summary>
    ///  Mono泛型单例抽象基类：懒汉式。不建议手动挂到场景：手动挂时子类不能写Start方法
    /// </summary>
    public abstract class SingletonMonoBase<T> : MonoBehaviour
    where T : SingletonMonoBase<T>
    {
        protected static T instance;//实例
        private static readonly object _lock = new object();//对象锁

        /// <summary>
        /// 单例实例化
        /// </summary>
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (_lock)
                    {
                        if (instance == null)
                        {
                            var gameObj = new GameObject(typeof(T).Name);
                            instance = gameObj.AddComponent<T>();
                            UnityEngine.Object.DontDestroyOnLoad(gameObj);
                            Debug.Log("[单例启动][Mono] : " + gameObj.name);
                            instance.OnInstance();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 单例实例化
        /// </summary>
        public static T GetInstance()
        {
            return Instance;
        }
        private void Start()
        {
            if (instance == null)
            {
                instance = this as T;
                UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
                Debug.Log("[单例启动][Mono] : " + this.gameObject.name);
                instance.OnInstance();
            }
        }

        public virtual void OnInstance() { }


    }








}