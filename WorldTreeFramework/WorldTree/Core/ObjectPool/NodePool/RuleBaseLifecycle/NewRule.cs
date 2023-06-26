
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/1 9:47

* 描述： 

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
}
