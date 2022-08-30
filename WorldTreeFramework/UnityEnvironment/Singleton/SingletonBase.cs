/****************************************

* 创 建 者：  闪电黑客
* 创建时间：  2022/5/7 9:23
* 描    述:

****************************************/


using UnityEngine;

namespace WorldTree
{
    /// <summary>
    /// 泛型单例基类：懒汉式
    /// </summary>
    public class SingletonBase<T>: Unit
    where T : SingletonBase<T>, new()
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
                            instance = new T();
                            Debug.Log("[单例启动] : " + typeof(T).Name);
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

        /// <summary>
        /// 单例实例时
        /// </summary>
        public virtual void OnInstance() { }
        public override void OnDispose()
        {
            instance = null;
        }

    }
}