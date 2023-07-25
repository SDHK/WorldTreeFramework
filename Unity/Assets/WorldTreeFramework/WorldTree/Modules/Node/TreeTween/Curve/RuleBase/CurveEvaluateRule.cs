
/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/16 15:57

* 描述： 

*/

namespace WorldTree
{
    /// <summary>
    /// 曲线执行法则接口
    /// </summary>
    public interface ICurveEvaluateRule : ICallRuleBase<float, float> { }

    /// <summary>
    /// 曲线执行法则
    /// </summary>
    public abstract class CurveEvaluateRule<N> : CallRuleBase<N, ICurveEvaluateRule, float, float> where N : class, INode, AsRule<ICurveEvaluateRule> { }
}
