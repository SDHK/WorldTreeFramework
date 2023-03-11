/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/18 9:35

* 描述： 节点监听法则基类
* 
* 主要作用： 给管理器用的节点添加和移除时的事件监听
* 
* 这样就不需要手动将节点添加到管理器，
* 在而是在添加或移除节点的时候，
* 管理器就能监听到指定节点类型的添加移除事件，并且拿到实例。
* 
* 而且监听移除事件，也能防止节点被移除后，管理器忘记手动移除的情况。
* 
* 设定：
* 1.静态指定节点类型。 
*   泛型填写目标节点类型，节点指定后，指定法则是无效的。
*   
* 2.静态指定法则类型。 
*   泛型填写目标节点必须为 INode，才生效
* 
* 3.动态指定。 
*   节点必须指定为 INode，法则必须指定为 IRule
*   可在运行时随意切换指定目标
*   
*   
* 
*/

using System;

namespace WorldTree
{

    /// <summary>
    /// 节点监听法则接口
    /// </summary>
    public interface IListenerRule : ISendRule<INode>
    {
        /// <summary>
        /// 监听：目标节点类型
        /// </summary>
        Type TargetNodeType { get; }
        /// <summary>
        /// 监听：目标节点法则
        /// </summary>
        Type TargetRuleType { get; }
    }


    /// <summary>
    /// 节点监听法则抽象基类
    /// </summary>
    public abstract class ListenerRuleBase<LN, LR, TN, TR> : RuleBase<LN, LR>, IListenerRule
    where LN : class, INode
    where TN : class, INode
    where LR : IListenerRule
    where TR : IRule
    {
        public virtual Type TargetNodeType => typeof(TN);
        public virtual Type TargetRuleType => typeof(TR);

        public virtual void Invoke(INode self, INode node) => OnEvent(self as LN, node as TN);
        public abstract void OnEvent(LN self, TN node);
    }
}
