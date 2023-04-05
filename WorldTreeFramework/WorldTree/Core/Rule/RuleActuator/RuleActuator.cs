/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 22:03

* 描述： 法则执行器
* 
* 用于执行拥有指定法则的节点
* 可用于代替委托事件
* 

*/

namespace WorldTree
{
    /// <summary>
    /// 法则执行器 接口基类
    /// </summary>
    public interface IRuleActuator { }

    /// <summary>
    /// 法则执行器 逆变泛型接口
    /// </summary>
    /// <typeparam name="T">法则类型</typeparam>
    /// <remarks>
    /// <para>主要通过法则类型逆变提示可填写参数</para>
    /// <para> RuleActuator 是没有泛型的实例，所以执行参数可能填错</para>
    /// </remarks>
    public interface IRuleActuator<in T> : IRuleActuator where T : IRule { }

    /// <summary>
    /// 法则执行器
    /// </summary>
    public partial class RuleActuator : Node, IAwake, ChildOf<INode>, IRuleActuator<IRule>
    {
        /// <summary>
        /// 法则集合
        /// </summary>
        public RuleGroup ruleGroup;

        /// <summary>
        /// 动态节点队列
        /// </summary>
        public DynamicNodeQueue nodeQueue;


        public int Count => nodeQueue is null ? 0 : nodeQueue.Count;


        public override string ToString()
        {
            return $"RuleActuator : {ruleGroup.RuleType}";
        }
    }
}
