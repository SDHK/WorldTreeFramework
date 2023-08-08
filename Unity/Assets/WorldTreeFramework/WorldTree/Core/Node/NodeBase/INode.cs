/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/11 16:39

* 描述： 树节点最底层接口
* 
* 抽出这个接口是为了用于扩展原生类型

*/
using System;

namespace WorldTree
{
    /// <summary>
    /// 默认类型，在泛型无法使用null时使用
    /// </summary>
    /// <remarks>直接使用default会造成反射创建和产生GC，所以存起来使用</remarks>
    public static class DefaultType<T>
    {
        /// <summary>
        /// 默认值
        /// </summary>
        public static readonly T Default = default;
    }

    /// <summary>
    /// 类型信息
    /// </summary>
    public static class TypeInfo<T>
    {
        public static readonly Type Type = typeof(T);
        public static readonly int HashCode = typeof(T).GetHashCode();
        public static readonly string TypeName = typeof(T).Name;
    }

    /// <summary>
    /// 组件：父节点限制
    /// </summary>
    /// <typeparam name="T">父节点类型</typeparam>
    /// <remarks>限制节点可挂的父节点，和Where约束搭配形成结构限制</remarks>
    public interface ComponentOf<in T> where T : class, INode { }

    /// <summary>
    /// 节点：父节点限制
    /// </summary>
    /// <typeparam name="T">父节点类型</typeparam>
    /// <remarks>限制节点可挂的父节点，和Where约束搭配形成结构限制</remarks>
    public interface ChildOf<in T> where T : class, INode { }


    /// <summary>
    /// 节点：可用法则限制
    /// </summary>
    /// <typeparam name="R">法则类型</typeparam>
    /// <remarks>节点拥有的法则，和Where约束搭配形成法则调用限制</remarks>
    public interface AsRule<in R> where R : IRule { }


    /// <summary>
    /// 核心节点标记
    /// </summary>
    /// <remarks>将节点标记为核心组件，避免核心启动时处理自己出现死循环</remarks>
    public interface ICoreNode { }

    /// <summary>
    /// 世界树节点接口
    /// </summary>
    /// <remarks>
    /// <para>世界树节点最底层接口</para> 
    /// </remarks>
    public interface INode : IUnitPoolItem

        , AsRule<INewRule>
        , AsRule<IGetRule>
        , AsRule<IRecycleRule>
        , AsRule<IDestroyRule>

        , AsRule<IEnableRule>
        , AsRule<IDisableRule>

        , AsRule<IAddRule>
        , AsRule<IUpdateRule>
        , AsRule<IRemoveRule>

        , AsRule<IDeReferencedChildRule>
        , AsRule<IDeReferencedParentRule>

        , AsRule<IReferencedChildRemoveRule>
        , AsRule<IReferencedParentRemoveRule>

    {

        /// <summary>
        /// 节点ID
        /// </summary>
        /// <remarks>在框架内唯一</remarks>
        public long Id { get; set; }

        /// <summary>
        /// 数据节点ID
        /// </summary>
        /// <remarks>保证数据唯一</remarks>
        public long DataId { get; set; }

        /// <summary>
        /// 节点类型
        /// </summary>
        public Type Type { get; set; }

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
        /// 活跃状态(设定为只读，禁止修改)
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


        #region Referenceds

        /// <summary>
        /// 引用我的父关系节点
        /// </summary>
        public UnitDictionary<long, INode> m_ReferencedParents { get; set; }

        /// <summary>
        /// 我引用的子关系节点
        /// </summary>
        public UnitDictionary<long, INode> m_ReferencedChilden { get; set; }

        #endregion


        #region Rule
        /// <summary>
        /// 法则列表字典
        /// </summary>
        public UnitDictionary<Type, IRuleList> m_RuleListDictionary { get; set; }
        #endregion

    }
}