/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/18 9:35

* 描述： 实体监听系统
* 
* 思路为给管理器用的实体添加事件的监听
* 
* 这样就不需要每增加一个管理器，
* 就得去注册管理器的监听。
* 
* 在而是在添加任意实体的时候，判断有实现系统的实体
* 就能监听全局的实体添加移除事件
* 
* 添加和移除时 不会监听到自己。
* 

*/

namespace WorldTree
{



    /// <summary>
    /// 监听系统
    /// </summary>
    public interface IEntitySystem : ISystem
    {
        void AddEntity(Entity self, Entity entity);
        void RemoveEntity(Entity self, Entity entity);
    }

    /// <summary>
    /// 实体监听系统
    /// </summary>
    public abstract class EntitySystem<T> : SystemBase<T,IEntitySystem>, IEntitySystem
        where T :  Entity
    {
        public void AddEntity(Entity self, Entity entity) => OnAddEntity(self as T, entity);

        public void RemoveEntity(Entity self, Entity entity) => OnRemoveEntity(self as T, entity);

        /// <summary>
        /// 实体添加时
        /// </summary>
        public abstract void OnAddEntity(T self, Entity entity);

        /// <summary>
        /// 实体移除时
        /// </summary>
        public abstract void OnRemoveEntity(T self, Entity entity);
    }


}
