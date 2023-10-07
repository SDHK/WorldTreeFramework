/****************************************

* 创 建 者：  闪电黑客
* 创建时间：  2022/5/6 21:27
* 描    述:   世界树节点基类

****************************************/

using System;

namespace WorldTree
{
    /// <summary>
    /// 世界树核心节点基类
    /// </summary>
    public abstract partial class CoreNode : Node, ICoreNode { }

    /// <summary>
    /// 世界树节点基类
    /// </summary>
    public abstract partial class Node : INode
    {
        public bool IsFromPool { get; set; }
        public bool IsRecycle { get; set; }

        public bool IsDisposed { get; set; }

        public long Id { get; set; }
        public long DataId { get; set; }

        public long Type { get; set; }

        public WorldTreeCore Core { get; set; }
        public WorldTreeRoot Root { get; set; }
        public INode Branch { get; set; }
        public INode Parent { get; set; }

        #region Active

        public bool ActiveToggle { get; set; }

        public bool IsActive { get; set; }

        public bool m_ActiveEventMark { get; set; }

        #endregion

        #region Children

        public UnitDictionary<long, INode> m_Children { get; set; }
        #endregion


        #region Component
        public bool isComponent { get; set; }
        public UnitDictionary<long, INode> m_Components { get; set; }
        #endregion

        #region Domains


        public UnitDictionary<Type, INode> m_Domains { get; set; }
        #endregion

        #region Referenceds

        public UnitDictionary<long, INode> m_ReferencedParents { get; set; }

        public UnitDictionary<long, INode> m_ReferencedChilden { get; set; }

        #endregion


        public override string ToString()
        {
            return GetType().ToString();
        }

        /// <summary>
        /// 回收节点
        /// </summary>
        public virtual void Dispose()
        {
            this.DisposeSelf();
        }

        /// <summary>
        /// 释放后：回收到对象池
        /// </summary>
        public virtual void OnDispose()
        {
            Core?.Recycle(this);
        }

    }

}
