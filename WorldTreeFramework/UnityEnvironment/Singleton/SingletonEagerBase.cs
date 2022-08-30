using WorldTree.SingletonAttribute;
using UnityEngine;


namespace WorldTree
{
    /// <summary>
    /// 泛型单例抽象基类：饿汉式(禁止手动实例)
    /// </summary>
    [SingletonEagerBaseAttribute]
    public abstract class SingletonEagerBase<T> : Unit, ISingletonEager
        where T : SingletonEagerBase<T>, new()
    {
        protected static T instance;//实例

        public void InitializeOnLoadMethod()
        {
            if (instance == null)
            {
                instance = (T)this;
                Debug.Log("[单例启动]（饿汉） : " + typeof(T).Name);
                instance.OnInstance();
            }
        }

        public virtual void OnInstance() { }


        public static T Instance { get => instance; }


    }
}
