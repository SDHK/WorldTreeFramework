using System;
using System.Collections.Generic;

namespace WorldTree
{
    public interface INode : IDisposable
    {
        #region 标记
        /// <summary>
        /// 回收标记
        /// </summary>
        public bool IsRecycle { get; set; }
        /// <summary>
        /// 释放标记
        /// </summary>
        public bool IsDisposed { get; set; }

        /// <summary>
        /// 组件标记
        /// </summary>
        public bool isComponent { get; set; }

        /// <summary>
        /// 节点ID
        /// </summary>
        /// <remarks>在框架内唯一</remarks>
        public long Id { get; set; }
        /// <summary>
        /// 节点类型
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// 活跃事件标记
        /// </summary>
        public bool ActiveEventMark { get; set; }

        /// <summary>
        /// 活跃状态
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 活跃标记
        /// </summary>
        public bool ActiveToggle { get; set; }


        /// <summary>
        /// 动态监听器状态
        /// </summary>
        public ListenerState listenerState { get; set; }

        /// <summary>
        /// 动态监听目标类型
        /// </summary>
        public Type listenerTarget { get; set; }

        #endregion

        #region 节点

        /// <summary>
        /// 根节点
        /// </summary>
        public WorldTreeRoot Root { get; set; }
        /// <summary>
        /// 父节点
        /// </summary>
        public INode Parent { get; set; }

        /// <summary>
        /// 组件节点
        /// </summary>
        public Dictionary<Type, INode> Components { get; set; }
        /// <summary>
        /// 子节点
        /// </summary>
        public UnitDictionary<long, INode> Children { get; set; }
        /// <summary>
        /// 域节点
        /// </summary>
        public UnitDictionary<Type, INode> Domains { get; set; }


        #endregion

        /// <summary>
        /// 对象释放时
        /// </summary>
        void OnDispose();
    }
}
