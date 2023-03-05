/****************************************

* 创 建 者：  闪电黑客
* 创建时间：  2022/5/6 21:31
* 
* 描    述: 规则基类
* 
* 是全部规则的基类

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
        /// 实体类型标记
        /// </summary>
        Type EntityType { get; }
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
    public abstract class RuleBase<E, R> : IRule
        where E : Node
        where R : IRule
    {
        public virtual Type EntityType => typeof(E);
        public virtual Type RuleType => typeof(R);
    }
}
