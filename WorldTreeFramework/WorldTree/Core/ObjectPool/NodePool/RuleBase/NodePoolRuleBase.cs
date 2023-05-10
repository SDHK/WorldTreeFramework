
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
    public interface INewRule : ISendRule { }

    /// <summary>
    /// 新建法则
    /// </summary>
    public abstract class NewRule<N> : SendRuleBase<N, INewRule> where N : class, INode { }

    /// <summary>
    /// 获取法则接口
    /// </summary>
    public interface IGetRule : ISendRule { }

    /// <summary>
    /// 获取法则
    /// </summary>
    public abstract class GetRule<N> : SendRuleBase<N, IGetRule> where N : class, INode { }

    /// <summary>
    /// 回收法则接口
    /// </summary>
    public interface IRecycleRule : ISendRule { }

    /// <summary>
    /// 回收法则
    /// </summary>
    public abstract class RecycleRule<N> : SendRuleBase<N, IRecycleRule> where N : class, INode { }

    /// <summary>
    /// 释放法则接口
    /// </summary>
    public interface IDestroyRule : ISendRule { }
    /// <summary>
    /// 释放法则
    /// </summary>
    public abstract class DestroyRule<N> : SendRuleBase<N, IDestroyRule> where N : class, INode { }
}
