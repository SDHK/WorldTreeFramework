﻿/****************************************

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

    public interface IEntityAddSystem : ISendSystem<Entity> { }
    public interface IEntityRemoveSystem : ISendSystem<Entity> { }

    /// <summary>
    /// 实体添加时
    /// </summary>
    public abstract class EntityAddSystem<T> : SystemBase<T, IEntityAddSystem>, IEntityAddSystem
        where T : Entity
    {
        public void Invoke(Entity self, Entity entity) => OnEntityAdd(self as T, entity);
        public abstract void OnEntityAdd(T self, Entity entity);
    }

    /// <summary>
    /// 实体移除时
    /// </summary>
    public abstract class EntityRemoveSystem<T> : SystemBase<T, IEntityRemoveSystem>, IEntityRemoveSystem
        where T : Entity
    {
        public void Invoke(Entity self, Entity entity) => OnEntityRemove(self as T, entity);
        public abstract void OnEntityRemove(T self, Entity entity);
    }
}
