/****************************************

* 作者： 闪电黑客
* 日期： 2023/8/30 19:23

* 描述： 

*/

namespace WorldTree
{
    /// <summary>
    /// 法则执行器 接口基类
    /// </summary>
    public interface IRuleActuator : INode
    {

        /// <summary>
        /// 刷新动态遍历数量
        /// </summary>
        public int RefreshTraversalCount();

        /// <summary>
        /// 尝试出列
        /// </summary>
        bool TryGetNext(out INode node, out RuleList ruleList);

        /// <summary>
        /// 尝试添加节点
        /// </summary>
        public bool TryAdd(INode node);

        /// <summary>
        /// 尝试添加节点，并建立引用关系
        /// </summary>
        public bool TryAddReferenced(INode node);

        /// <summary>
        /// 尝试添加节点与对应法则
        /// </summary>
        public bool TryAdd(INode node, RuleList ruleList);

        /// <summary>
        /// 尝试添加节点与对应法则，并建立引用关系
        /// </summary>
        public bool TryAddReferenced(INode node, RuleList ruleList);

        /// <summary>
        /// 移除节点
        /// </summary>
        public void Remove(long id);
        /// <summary>
        /// 移除节点
        /// </summary>
        public void Remove(INode node);

        /// <summary>
        /// 清除
        /// </summary>
        public void Clear();
    }

    /// <summary>
    /// 法则执行器 逆变泛型接口
    /// </summary>
    /// <typeparam name="T">法则类型</typeparam>
    /// <remarks>
    /// <para>主要通过法则类型逆变提示可填写参数</para>
    /// </remarks>
    public interface IRuleActuator<in T> : IRuleActuator where T : IRule { }


}
