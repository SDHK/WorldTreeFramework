/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/16 22:03

* 描述： 法则执行器
* 
* 用于执行拥有指定法则的节点
* 可用于代替委托事件
* 
* ！！！！RuleGroup 应该在每次使用时去RuleManager查询获取
* 为了方便切换

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
    public partial class RuleActuator : Node, ChildOf<INode>, IRuleActuator<IRule>
    {
        /// <summary>
        /// 法则集合
        /// </summary>
        public RuleGroup ruleGroup;

        /// <summary>
        /// 动态节点队列
        /// </summary>
        public DynamicNodeQueue nodeQueue;

        /// <summary>
        /// 当前数量
        /// </summary>
        public int Count => nodeQueue.Count;


        public override string ToString()
        {
            return $"RuleActuator : {ruleGroup.RuleType}";
        }


        /// <summary>
        /// 添加节点
        /// </summary>
        public void Enqueue(INode node)
        {
            nodeQueue.Enqueue(node);
        }

        /// <summary>
        /// 移除节点
        /// </summary>
        public void Remove(INode node)
        {
            nodeQueue.Remove(node);
        }
        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            nodeQueue.Clear();
        }

        /// <summary>
        /// 出列
        /// </summary>
        public INode Dequeue()
        {
            return nodeQueue.Dequeue();
        }

        /// <summary>
        /// 尝试出列
        /// </summary>
        public bool TryDequeue(out INode node)
        {
            return nodeQueue.TryDequeue(out node);
        }
    }

    class RuleActuatorAddRule : AddRule<RuleActuator>
    {
        public override void OnEvent(RuleActuator self)
        {

            self.AddComponent(out self.nodeQueue);

        }
    }
}
