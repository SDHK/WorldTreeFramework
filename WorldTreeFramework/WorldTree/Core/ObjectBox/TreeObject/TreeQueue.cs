﻿
/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 10:51

* 描述： 树泛型队列

*/

using System;
using System.Collections.Generic;

namespace WorldTree
{
    /// <summary>
    /// 树泛型队列
    /// </summary>
    public class TreeQueue<T> : Queue<T>, INode, ChildOf<INode>
        ,AsRule<IAwakeRule>
    {
        public bool IsRecycle { get; set; }
        public bool IsDisposed { get; set; }
        public long Id { get; set; }
        public Type Type { get; set; }
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
        public UnitDictionary<Type, INode> m_Components { get; set; }
        #endregion

        #region Domains

        public UnitDictionary<Type, INode> m_Domains { get; set; }
        #endregion

        #region Referenceds

        public UnitDictionary<long, INode> m_ReferencedParents { get; set; }

        public UnitDictionary<long, INode> m_ReferencedChilden { get; set; }

        #endregion

        #region Listener
        public ListenerState listenerState { get; set; } = ListenerState.Not;
        public Type listenerTarget { get; set; }
        #endregion


        public void Dispose()
        {
            this.DisposeSelf();
        }

        public virtual void OnDispose()
        {
            Clear();
            Core?.Recycle(this);
        }
    }




}
