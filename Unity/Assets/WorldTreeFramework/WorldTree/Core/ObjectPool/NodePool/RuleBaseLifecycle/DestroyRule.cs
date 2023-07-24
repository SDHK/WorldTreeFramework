
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/1 9:47

* 描述： 

*/

namespace WorldTree
{
    /// <summary>
    /// 释放法则接口
    /// </summary>
    public interface IDestroyRule : ISendRuleBase { }

    /// <summary>
    /// 释放法则
    /// </summary>
    public abstract class DestroyRule<N> : SendRuleBase<N, IDestroyRule> where N : class, INode, AsRule<IDestroyRule> { }
}
