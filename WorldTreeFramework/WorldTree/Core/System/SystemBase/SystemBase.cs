/****************************************

* 创 建 者：  闪电黑客
* 创建时间：  2022/5/6 21:31
* 
* 描    述: 系统基类
* 
* 是全部系统的基类

*/

using System;


namespace WorldTree
{
    /// <summary>
    /// 实体系统接口
    /// </summary>
    /// <remarks>
    /// <para>所有系统的最底层接口</para>
    /// <para>主要作用是可以让所有系统统一类型获取标记</para>
    /// </remarks>
    public interface IEntitySystem
    {
        /// <summary>
        /// 实体类型标记
        /// </summary>
        Type EntityType { get; }
        /// <summary>
        /// 系统类型标记
        /// </summary>
        Type SystemType { get; }
    }

    /// <summary>
    /// 系统抽象基类
    /// </summary>
    /// <remarks>
    /// <para>系统的最底层基类</para>
    /// <para>主要作用是通过泛型给标记赋值</para>         
    /// </remarks>
    public abstract class SystemBase<E, S> : IEntitySystem
        where E : Entity
        where S : IEntitySystem
    {
        public virtual Type EntityType => typeof(E);
        public virtual Type SystemType => typeof(S);
    }
}
