/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/18 9:35

* 描述： 实体监听系统事件基类
* 
* 主要作用： 给管理器用的实体添加和移除时的事件监听
* 
* 这样就不需要手动将实体添加到管理器，
* 在而是在添加或移除实体的时候，
* 管理器就能监听到指定实体类型的添加移除事件，并且拿到实例。
* 
* 而且监听移除事件，也能防止实体被移除后，管理器忘记手动移除的情况。
* 
* 设定：
* 1.静态指定实体类型。 
*   泛型填写目标实体类型，实体指定后，指定系统是无效的。
*   
* 2.静态指定系统类型。 
*   泛型填写目标实体必须为Entity，才生效
* 
* 3.动态指定。 
*   实体必须指定为Entity，系统必须指定为 IRule
*   可在运行时随意切换指定目标
*   
*   
* 
*/

using System;

namespace WorldTree
{

    /// <summary>
    /// 实体监听系统接口
    /// </summary>
    public interface IListenerSystem : ISendSystem<Node>
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


    /// <summary>
    /// 实体监听系统抽象基类
    /// </summary>
    public abstract class ListenerSystemBase<LE, LS, TE, TS> : RuleBase<LE, LS>, IListenerSystem
    where LE : Node
    where TE : Node
    where LS : IListenerSystem
    where TS : IRule
    {
        public virtual Type TargetEntityType => typeof(TE);
        public virtual Type TargetSystemType => typeof(TS);

        public virtual void Invoke(Node self, Node entity) => OnEvent(self as LE, entity as TE);
        public abstract void OnEvent(LE self, TE entity);
    }
}
