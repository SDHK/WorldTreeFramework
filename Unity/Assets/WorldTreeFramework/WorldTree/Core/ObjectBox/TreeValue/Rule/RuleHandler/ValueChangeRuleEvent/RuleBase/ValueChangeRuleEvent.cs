
/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/4 17:20

* 描述： 数值变化监听事件法则

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 泛型数值监听接口，用于解除AsRule的法则限制
    /// </summary>
    public interface IValueChangeRuleEvent : IRule { }

    /// <summary>
    /// 数值变化监听事件法则接口
    /// </summary>
    public interface IValueChangeRuleEvent<T1> : ISendRuleBase<T1>, IValueChangeRuleEvent { }

    /// <summary>
    /// 数值变化监听事件法则(同类型转换)
    /// </summary>
    public abstract class ValueChangeRuleEvent<N, T1> : SendRuleBase<N, IValueChangeRuleEvent<T1>, T1>
        where N : class, INode, AsRule<IValueChangeRuleEvent<T1>>
        where T1 : IEquatable<T1>
    { }

    /// <summary>
    /// 数值变化监听事件法则(同类型转换)
    /// </summary>
    public abstract class TreeValueChangeRuleEvent<T1> : ValueChangeRuleEvent<TreeValueBase<T1>, T1>
        where T1 : IEquatable<T1>
    { }

    /// <summary>
    /// 数值变化监听事件法则(不同类型转换)
    /// </summary>
    public abstract class TreeValueGenericsChangeRuleEvent<T1, T2> : ValueChangeRuleEvent<TreeValueBase<T1>, T2>
        where T1 : IEquatable<T1>
        where T2 : IEquatable<T2>
    { }






}
