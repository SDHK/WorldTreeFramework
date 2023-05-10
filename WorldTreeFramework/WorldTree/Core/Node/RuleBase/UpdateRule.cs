/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 23:26

* 描述： 刷新法则

*/
namespace WorldTree
{
    /// <summary>
    /// 刷新法则接口
    /// </summary>
    public interface IUpdateRule : ISendRule<float> { }

    /// <summary>
    /// 刷新法则
    /// </summary>
    public abstract class UpdateRule<N> : SendRuleBase<N, IUpdateRule, float> where N : class, INode { }
}
