/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/1 9:47

* 描述： 

*/

namespace WorldTree
{
    /// <summary>
    /// 回收法则接口
    /// </summary>
    public interface IRecycleRule : ISendRuleBase { }

    /// <summary>
    /// 回收法则
    /// </summary>
    public abstract class RecycleRule<N> : SendRuleBase<N, IRecycleRule> where N : class, INode, AsRule<IRecycleRule> { }
}
