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
    /// 系统接口
    /// </summary>
    public interface ISystem
    {
        Type EntityType { get; }
        Type SystemType { get; }
    }

    /// <summary>
    /// 系统基类
    /// </summary>
    public abstract class SystemBase<T,S> : ISystem
    {
        public Type EntityType => typeof(T);
        public Type SystemType => typeof(S);
    }
}
