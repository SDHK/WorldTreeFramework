/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/1 9:47

* 描述： 

*/

namespace WorldTree
{
    /// <summary>
    /// 获取法则接口
    /// </summary>
    public interface IGetRule : ISendRuleBase { }

    /// <summary>
    /// 获取法则
    /// </summary>
    public abstract class GetRule<N> : SendRuleBase<N, IGetRule> where N : class, INode, AsRule<IGetRule> { }
}
