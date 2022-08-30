using WorldTree.SingletonAttribute;
using UnityEngine;

namespace WorldTree
{
    /// <summary>
    /// Mono泛型单例抽象基类：饿汉式(禁止手动挂载)
    /// </summary>
    [SingletonMonoEagerBaseAttribute]
    public abstract class SingletonMonoEagerBase<T> : MonoBehaviour, ISingletonEager
        where T : SingletonMonoEagerBase<T>
    {
        protected static T instance;//实例

        public void InitializeOnLoadMethod()
        {
            if (instance == null)
            {
                instance = (T)this;
                Debug.Log("[单例启动][Mono]（饿汉） : " + typeof(T).Name);
                instance.OnInstance();
            }
        }
        public virtual void OnInstance() { }

        public static T Instance { get => instance; }
    }
}
