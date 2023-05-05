﻿
/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/4 17:20

* 描述： 数值变化监听法则

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 数值变化监听法则接口
    /// </summary>
    public interface IValueChangeRule<T1> : ISendRule<T1> { }
    /// <summary>
    /// 数值变化监听法则(同类型转换)
    /// </summary>
    public abstract class ValueChangeRule<N, T1> : SendRuleBase<IValueChangeRule<T1>, N, T1>
        where N : TreeValueBase<T1>
        where T1 : IEquatable<T1>
    { }

    /// <summary>
    /// 数值变化监听法则(不同类型转换)
    /// </summary>
    public abstract class ValueChangeRule<N, T1, T2> : SendRuleBase<IValueChangeRule<T2>, N, T2>
        where N : TreeValueBase<T1>
        where T1 : IEquatable<T1>
        where T2 : IEquatable<T2>
    { }


}
