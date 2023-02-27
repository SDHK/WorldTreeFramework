/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/21 21:12

* 描述： 初始化系统
* 
* 用于Add添加到实体树上时的构造参数传递
* 
* 在OnGet与OnAdd之间执行

*/

namespace WorldTree
{
    public interface IAwakeSystem : ISendSystem { }

    public interface IAwakeSystem<T1> : ISendSystem<T1> { }

    public interface IAwakeSystem<T1, T2> : ISendSystem<T1, T2> { }

    public interface IAwakeSystem<T1, T2, T3> : ISendSystem<T1, T2, T3> { }

    public interface IAwakeSystem<T1, T2, T3, T4> : ISendSystem<T1, T2, T3, T4> { }

    public interface IAwakeSystem<T1, T2, T3, T4, T5> : ISendSystem<T1, T2, T3, T4, T5> { }

    /// <summary>
    /// 初始化系统
    /// </summary>
    public abstract class AwakeSystem<E> : SendSystemBase<IAwakeSystem, E> where E : Entity { }
    /// <summary>
    /// 初始化系统
    /// </summary>
    public abstract class AwakeSystem<E, T1> : SendSystemBase<IAwakeSystem<T1>, E, T1> where E : Entity { }
    /// <summary>
    /// 初始化系统
    /// </summary>
    public abstract class AwakeSystem<E, T1, T2> : SendSystemBase<IAwakeSystem<T1, T2>, E, T1, T2> where E : Entity { }
    /// <summary>
    /// 初始化系统
    /// </summary>
    public abstract class AwakeSystem<E, T1, T2, T3> : SendSystemBase<IAwakeSystem<T1, T2, T3>, E, T1, T2, T3> where E : Entity { }
    /// <summary>
    /// 初始化系统
    /// </summary>
    public abstract class AwakeSystem<E, T1, T2, T3, T4> : SendSystemBase<IAwakeSystem<T1, T2, T3, T4>, E, T1, T2, T3, T4> where E : Entity { }
    /// <summary>
    /// 初始化系统
    /// </summary>
    public abstract class AwakeSystem<E, T1, T2, T3, T4, T5> : SendSystemBase<IAwakeSystem<T1, T2, T3, T4, T5>, E, T1, T2, T3, T4, T5> where E : Entity { }


}
