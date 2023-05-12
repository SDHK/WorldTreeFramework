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
    /// 引用子关系解除法则接口
    /// </summary>
    public interface IDeReferencedChildRule : ISendRule<INode> { }

    /// <summary>
    /// 引用子关系解除法则
    /// </summary>
    public abstract class DeReferencedChildRule<N> : SendRuleBase<N, IDeReferencedChildRule, INode> where N : class, INode { }


    /// <summary>
    /// 引用父关系解除法则接口
    /// </summary>
    public interface IDeReferencedParentRule : ISendRule<INode> { }

    /// <summary>
    /// 引用父关系解除法则
    /// </summary>
    public abstract class DeReferencedParentRule<N> : SendRuleBase<N, IDeReferencedParentRule, INode> where N : class, INode { }



    /// <summary>
    /// 引用子关系移除法则接口
    /// </summary>
    public interface IReferencedChildRemoveRule : ISendRule<INode> { }
    /// <summary>
    /// 引用子关系移除法则
    /// </summary>
    public abstract class ReferencedChildRemoveRule<N> : SendRuleBase<N, IDeReferencedChildRule, INode> where N : class, INode { }


    /// <summary>
    /// 引用父关系移除法则接口
    /// </summary>
    public interface IReferencedParentRemoveRule : ISendRule<INode> { }

    /// <summary>
    /// 引用子关系移除法则
    /// </summary>
    public abstract class ReferencedParentRemoveRule<N> : SendRuleBase<N, IDeReferencedChildRule, INode> where N : class, INode { }


}
