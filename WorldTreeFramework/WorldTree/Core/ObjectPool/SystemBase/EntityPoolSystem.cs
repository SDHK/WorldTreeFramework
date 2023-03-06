
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
    public abstract class NewRule<E> : SendRuleBase<INewRule, E> where E : Node { }

    /// <summary>
    /// 获取法则接口
    /// </summary>
    public interface IGetRule : ISendRule { }

    /// <summary>
    /// 获取法则
    /// </summary>
    public abstract class GetRule<E> : SendRuleBase<IGetRule, E> where E : Node { }

    /// <summary>
    /// 回收法则接口
    /// </summary>
    public interface IRecycleRule : ISendRule { }

    /// <summary>
    /// 回收法则
    /// </summary>
    public abstract class RecycleRule<E> : SendRuleBase<IRecycleRule, E> where E : Node { }

    /// <summary>
    /// 释放法则接口
    /// </summary>
    public interface IDestroyRule : ISendRule { }
    /// <summary>
    /// 释放法则
    /// </summary>
    public abstract class DestroySystem<E> : SendRuleBase<IDestroyRule, E> where E : Node { }
}
