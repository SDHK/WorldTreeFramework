
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/1 9:47

* 描述： 添加法则
* 
* 在加入节点时触发事件

*/

namespace WorldTree
{
    /// <summary>
    /// 添加法则接口
    /// </summary>
    public interface IAddRule : ISendRule { }

    /// <summary>
    /// 添加法则
    /// </summary>
    public abstract class AddRule<N> : SendRuleBase<IAddRule, N> where N : class,INode { }
}
