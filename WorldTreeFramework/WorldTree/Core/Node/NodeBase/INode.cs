/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/11 16:39

* 描述： 世界树节点最底层接口
* 
* 抽出这个接口是为了用于扩展原生类型

*/
using System;

namespace WorldTree
{

    /// <summary>
    /// 组件：父节点限制
    /// </summary>
    /// <typeparam name="T">父节点类型</typeparam>
    public interface ComponentOf<in T> where T : class, INode { }


    /// <summary>
    /// 节点：父节点限制
    /// </summary>
    /// <typeparam name="T">父节点类型</typeparam>
    public interface ChildOf<in T> where T : class, INode { }


    /// <summary>
    /// 监听器状态
    /// </summary>
    public enum ListenerState
    {
        /// <summary>
        /// 不是监听器
        /// </summary>
        Not,
        /// <summary>
        /// 监听目标是节点
        /// </summary>
        Node,
        /// <summary>
        /// 监听目标是法则
        /// </summary>
        Rule
    }

    /// <summary>
    /// 不被监听的类型
    /// </summary>
    public interface INotListenedNode { }

    /// <summary>
    /// 世界树节点接口
    /// </summary>
    /// <remarks>
    /// <para>世界树节点最底层接口</para> 
    /// </remarks>
    public interface INode : IUnitPoolItem
    {

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
        /// 树心节点
        /// </summary>
        /// <remarks>挂载框架的启动核心组件</remarks>
        public WorldTreeCore Core { get; set; }

        /// <summary>
        /// 树根节点
        /// </summary>
        /// <remarks>挂载核心启动后的管理器组件</remarks>
        public WorldTreeRoot Root { get; set; }

        /// <summary>
        /// 树枝节点
        /// </summary>
        /// <remarks>用于划分作用域</remarks>
        public INode Branch { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        public INode Parent { get; set; }


        #region Active
        /// <summary>
        /// 活跃开关
        /// </summary>
        public bool ActiveToggle { get; set; }

        /// <summary>
        /// 活跃状态
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 活跃事件标记
        /// </summary>
        public bool m_ActiveEventMark { get; set; }

        #endregion

        #region Children

        /// <summary>
        /// 子节点
        /// </summary>
        public UnitDictionary<long, INode> m_Children { get; set; }

        #endregion

        #region Components

        /// <summary>
        /// 组件标记
        /// </summary>
        public bool isComponent { get; set; }

        /// <summary>
        /// 组件节点
        /// </summary>
        public UnitDictionary<Type, INode> m_Components { get; set; }
        #endregion


        #region Domains



        /// <summary>
        /// 域节点
        /// </summary>
        public UnitDictionary<Type, INode> m_Domains { get; set; }

        #endregion

        #region Listener
        /// <summary>
        /// 动态监听器状态
        /// </summary>
        public ListenerState listenerState { get; set; }

        /// <summary>
        /// 动态监听目标类型
        /// </summary>
        public Type listenerTarget { get; set; }
        #endregion

    }
}
