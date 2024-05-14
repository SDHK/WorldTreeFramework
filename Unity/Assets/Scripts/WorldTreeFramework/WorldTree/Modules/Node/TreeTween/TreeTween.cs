
/****************************************

* 作者： 闪电黑客
* 日期： 2022/12/14 15:34

* 描述： 树渐变节点

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 值渐变
    /// </summary>
    public class TreeTween<T1> : TreeTweenBase, ComponentOf<TreeValueBase<T1>>
        , AsRule<IAwakeRule>
        , AsRule<TweenUpdate>
        where T1 : IEquatable<T1>
    {
        /// <summary>
        /// 值
        /// </summary>
        public TreeValueBase<T1> changeValue;

        /// <summary>
        /// 开始
        /// </summary>
        public TreeValueBase<T1> startValue;

        /// <summary>
        /// 结束
        /// </summary>
        public TreeValueBase<T1> endValue;
    }
}
