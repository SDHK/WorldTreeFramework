/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/4 14:43

* 描述： 引用池管理器

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 引用池
    /// </summary>
    public class ReferencedPool : TreeDictionary<long, INode>, ICoreNode, ChildOf<ReferencedPoolManager>
    {
        /// <summary>
        /// 引用池类型
        /// </summary>
        public Type ReferencedType;
        public override string ToString()
        {
            return $"ReferencedPool<{ReferencedType}>:[{Count}]";
        }
    }
}
