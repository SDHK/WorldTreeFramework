/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/31 18:06

* 描述： 引用解除法则
* 
* 在我引用的节点回收时触发事件

*/

namespace WorldTree
{
    /// <summary>
    /// 引用解除法则接口
    /// </summary>
    public interface IReferencedRemoveRule : ISendRule<INode> { }

    /// <summary>
    /// 引用解除法则
    /// </summary>
    public abstract class ReferencedRemoveRule<N> : SendRuleBase<IReferencedRemoveRule, N, INode> where N : class, INode { }

}
