/****************************************

* 创 建 者：  闪电黑客
* 创建时间：  2022/5/6 21:31
* 
* 描    述: 法则基类
* 
* 是全部法则的基类

*/

using System;


namespace WorldTree
{
    /// <summary>
    /// 法则接口
    /// </summary>
    /// <remarks>
    /// <para>所有法则的最底层接口</para>
    /// <para>主要作用是可以让所有法则统一类型获取标记</para>
    /// </remarks>
    public interface IRule
    {
        /// <summary>
        /// 节点类型标记
        /// </summary>
        Type NodeType { get; }
        /// <summary>
        /// 法则类型标记
        /// </summary>
        Type RuleType { get; }
    }

    /// <summary>
    /// 法则抽象基类
    /// </summary>
    /// <remarks>
    /// <para>法则的最底层基类</para>
    /// <para>主要作用是通过泛型给标记赋值</para>         
    /// </remarks>
    public abstract class RuleBase<N, R> : IRule
        where N : class, INode, AsRule<R>
        where R : IRule
    {
        public virtual Type NodeType => typeof(N);
        public virtual Type RuleType => typeof(R);
    }
}
