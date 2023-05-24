﻿
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/1 9:47

* 描述： 实体对象池的法则

*/

namespace WorldTree
{
    /// <summary>
    /// 新建法则接口
    /// </summary>
    public interface INewRule : ISendRuleBase { }

    /// <summary>
    /// 新建法则
    /// </summary>
    public abstract class NewRule<N> : SendRuleBase<N, INewRule> where N : class, INode, AsRule<INewRule> { }

    /// <summary>
    /// 获取法则接口
    /// </summary>
    public interface IGetRule : ISendRuleBase { }

    /// <summary>
    /// 获取法则
    /// </summary>
    public abstract class GetRule<N> : SendRuleBase<N, IGetRule> where N : class, INode, AsRule<IGetRule> { }

    /// <summary>
    /// 回收法则接口
    /// </summary>
    public interface IRecycleRule : ISendRuleBase { }

    /// <summary>
    /// 回收法则
    /// </summary>
    public abstract class RecycleRule<N> : SendRuleBase<N, IRecycleRule> where N : class, INode, AsRule<IRecycleRule> { }

    /// <summary>
    /// 释放法则接口
    /// </summary>
    public interface IDestroyRule : ISendRuleBase { }
    /// <summary>
    /// 释放法则
    /// </summary>
    public abstract class DestroyRule<N> : SendRuleBase<N, IDestroyRule> where N : class, INode, AsRule<IDestroyRule> { }
}
