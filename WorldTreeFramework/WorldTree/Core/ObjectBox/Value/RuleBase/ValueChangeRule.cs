
/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/4 17:20

* 描述： 数值变化监听法则

*/

namespace WorldTree
{
    /// <summary>
    /// 数值变化监听法则接口
    /// </summary>
    public interface IValueChangeRule<T1> : ISendRule<T1> { }
    /// <summary>
    /// 数值变化监听法则
    /// </summary>
    public abstract class ValueChangeRule<N, T1> : SendRuleBase<IValueChangeRule<T1>, N, T1>
        where N : class, ITreeValue<T1>, INode
    { }

}
