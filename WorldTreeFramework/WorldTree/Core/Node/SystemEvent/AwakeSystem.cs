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
    /// <summary>
    /// 初始化系统
    /// </summary>
    public interface IAwakeSystem : ISendRule { }
    /// <summary>
    /// 初始化系统
    /// </summary>
    public interface IAwakeSystem<T1> : ISendRule<T1> { }
    /// <summary>
    /// 初始化系统
    /// </summary>
    public interface IAwakeSystem<T1, T2> : ISendRule<T1, T2> { }
    /// <summary>
    /// 初始化系统
    /// </summary>
    public interface IAwakeSystem<T1, T2, T3> : ISendRule<T1, T2, T3> { }
    /// <summary>
    /// 初始化系统
    /// </summary>
    public interface IAwakeSystem<T1, T2, T3, T4> : ISendRule<T1, T2, T3, T4> { }
    /// <summary>
    /// 初始化系统
    /// </summary>
    public interface IAwakeSystem<T1, T2, T3, T4, T5> : ISendRule<T1, T2, T3, T4, T5> { }


    /// <summary>
    /// 初始化系统事件
    /// </summary>
    public abstract class AwakeSystem<E> : SendRuleBase<IAwakeSystem, E> where E : Node { }
    /// <summary>
    /// 初始化系统事件
    /// </summary>
    public abstract class AwakeSystem<E, T1> : SendRuleBase<IAwakeSystem<T1>, E, T1> where E : Node { }
    /// <summary>
    /// 初始化系统事件
    /// </summary>
    public abstract class AwakeSystem<E, T1, T2> : SendRuleBase<IAwakeSystem<T1, T2>, E, T1, T2> where E : Node { }
    /// <summary>
    /// 初始化系统事件
    /// </summary>
    public abstract class AwakeSystem<E, T1, T2, T3> : SendRuleBase<IAwakeSystem<T1, T2, T3>, E, T1, T2, T3> where E : Node { }
    /// <summary>
    /// 初始化系统事件
    /// </summary>
    public abstract class AwakeSystem<E, T1, T2, T3, T4> : SendRuleBase<IAwakeSystem<T1, T2, T3, T4>, E, T1, T2, T3, T4> where E : Node { }
    /// <summary>
    /// 初始化系统事件
    /// </summary>
    public abstract class AwakeSystem<E, T1, T2, T3, T4, T5> : SendRuleBase<IAwakeSystem<T1, T2, T3, T4, T5>, E, T1, T2, T3, T4, T5> where E : Node { }


}
