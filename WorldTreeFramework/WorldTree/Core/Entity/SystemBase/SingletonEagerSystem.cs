/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/18 9:35

* 描述： ECS模式的单例系统
* 实现思路为给根节点挂组件
* 

*/

namespace WorldTree
{
    /// <summary>
    /// 实体饿汉单例系统接口
    /// </summary>
    public interface ISingletonEagerSystem : ISystem
    {
        void Singleton(EntityManager domain);
    }

    /// <summary>
    /// 实体饿汉单例系统：生成组件挂在根节点下
    /// </summary>
    public abstract class SingletonEagerSystem<T> : SystemBase<T, ISingletonEagerSystem>, ISingletonEagerSystem
        where T :Entity
    {
        public void Singleton(EntityManager Root)
        {
            Root.AddComponent<T>();
        }
    }
}
