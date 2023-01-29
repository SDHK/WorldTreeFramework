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
* 监听系统属于实体，由对象池使用，通过监听泛型去取管理器参数
* 思考：监听系统组
*/

using System;

namespace WorldTree
{

    /// <summary>
    /// 监听系统接口
    /// </summary>
    public interface IListenerSystem : ISendSystem< Entity>
    {
        /// <summary>
        /// 监听：目标实体类型
        /// </summary>
        Type TargetEntityType { get; }
        /// <summary>
        /// 监听：目标实体系统
        /// </summary>
        Type TargetSystemType { get; }
    }
    public interface IEntityAddSystem : IListenerSystem { }
    public interface IEntityRemoveSystem : IListenerSystem { }

    /// <summary>
    /// 监听系统抽象基类
    /// </summary>
    public abstract class ListenerSystemBase<LE, LS, TE, TS> : SystemBase<LE, LS>
    {
        public virtual Type TargetEntityType => typeof(TE);
        public virtual Type TargetSystemType => typeof(TS);
    }

    /// <summary>
    /// 实体添加时
    /// </summary>
    public abstract class EntityAddSystem<LE, TE, TS> : ListenerSystemBase<LE, IEntityAddSystem, TE, TS>, IEntityAddSystem
        where TE : Entity
        where LE : Entity
        where TS : ISystem
    {
        public void Invoke(Entity self, Entity entity) => OnEntityAdd(self as LE, entity as TE);
        public abstract void OnEntityAdd(LE self, TE entity);
    }

    /// <summary>
    /// 实体移除时
    /// </summary>
    public abstract class EntityRemoveSystem<LE, TE, TS> : ListenerSystemBase<LE, IEntityRemoveSystem, TE, TS>, IEntityRemoveSystem
        where TE : Entity
        where LE : Entity
        where TS : ISystem
    {
        public void Invoke(Entity self, Entity entity) => OnEntityRemove(self as LE, entity as TE);
        public abstract void OnEntityRemove(LE self, TE entity);
    }


    /// <summary>
    /// 实体添加时
    /// </summary>
    public abstract class EntityAddSystem<LE> : ListenerSystemBase<LE, IEntityAddSystem, Entity, ISystem>, IEntityAddSystem
        where LE : Entity
    {
        public void Invoke(Entity self, Entity entity) => OnEntityAdd(self as LE, entity);
        public abstract void OnEntityAdd(LE self, Entity entity);
    }


    /// <summary>
    /// 实体移除时
    /// </summary>
    public abstract class EntityRemoveSystem<LE> : ListenerSystemBase<LE, IEntityRemoveSystem, Entity, ISystem>, IEntityRemoveSystem
        where LE : Entity
    {
        public void Invoke(Entity self, Entity entity) => OnEntityRemove(self as LE, entity);
        public abstract void OnEntityRemove(LE self, Entity entity);
    }
}
