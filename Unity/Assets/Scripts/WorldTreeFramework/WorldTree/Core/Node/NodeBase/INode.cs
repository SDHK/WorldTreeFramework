/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/11 16:39

* 描述： 树节点最底层接口
*
* 抽出这个接口是为了用于扩展原生类型

*/

namespace WorldTree
{
	/// <summary>
	/// 世界树节点可视化生成器接口
	/// </summary>
	public interface IWorldTreeNodeViewBuilder : INode
	{ }

	/// <summary>
	/// 世界树节点接口
	/// </summary>
	/// <remarks>
	/// <para>世界树节点最底层接口</para>
	/// </remarks>
	public partial interface INode : IUnitPoolItem
		, AsRule<New>
		, AsRule<Get>
		, AsRule<Recycle>
		, AsRule<Destroy>

		, AsRule<Enable>
		, AsRule<Disable>

		, AsRule<Graft>
		, AsRule<Cut>

		, AsRule<Add>
		, AsRule<Update>
		, AsRule<UpdateTime>
		, AsRule<BeforeRemove>
		, AsRule<Remove>
	{
		/// <summary>
		/// 节点ID
		/// </summary>
		/// <remarks>递增ID，只在每个框架实例内唯一</remarks>
		public long Id { get; set; }

		/// <summary>
		/// 树根节点
		/// </summary>
		/// <remarks>挂载核心启动后的管理器组件</remarks>
		public WorldTreeRoot Root { get; set; }

		/// <summary>
		/// 树枝节点
		/// </summary>
		/// <remarks>用于划分作用域</remarks>
		public INode Domain { get; set; }

		/// <summary>
		/// 父节点
		/// </summary>
		public INode Parent { get; set; }

		/// <summary>
		/// 可视化生成器
		/// </summary>
		public IWorldTreeNodeViewBuilder View { get; set; }

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
		/// 活跃事件标记，这个由框架内部调用设置，禁止修改
		/// </summary>
		public bool m_ActiveEventMark { get; set; }

		/// <summary>
		/// 设置当前节点激活状态
		/// </summary>
		public void SetActive(bool value);

		/// <summary>
		/// 刷新当前节点激活状态：层序遍历设置子节点
		/// </summary>
		public void RefreshActive();

		#endregion

		#region Rattan

		/// <summary>
		/// 树藤分支
		/// </summary>
		public UnitDictionary<long, IRattan> m_Rattans { get; set; }

		/// <summary>
		/// 树藤分支
		/// </summary>
		public UnitDictionary<long, IRattan> Rattans { get; }

		#endregion

		#region Branch

		/// <summary>
		/// 此节点挂载到父级的分支类型
		/// </summary>
		public long BranchType { get; set; }

		/// <summary>
		/// 树分支
		/// </summary>
		public UnitDictionary<long, IBranch> m_Branchs { get; set; }

		/// <summary>
		/// 树分支
		/// </summary>
		public UnitDictionary<long, IBranch> Branchs { get; }

		#endregion

		#region 节点处理

		#region 添加

		/// <summary>
		/// 尝试添加到树结构上
		/// </summary>
		public bool TryAddSelfToTree<B, K>(K Key, INode parent) where B : class, IBranch<K>;

		/// <summary>
		/// 节点加入树结构时的处理
		/// </summary>
		/// <remarks>由框架内部调用</remarks>
		public void OnAddSelfToTree();

		#endregion

		#region 嫁接

		/// <summary>
		/// 节点嫁接到树结构
		/// </summary>
		public bool TryGraftSelfToTree<B, K>(K key, INode parent) where B : class, IBranch<K>;

		/// <summary>
		/// 节点嫁接到树结构时的处理
		/// </summary>
		/// <remarks>由框架内部调用</remarks>
		public void OnGraftSelfToTree();

		#endregion

		#region 裁剪

		/// <summary>
		/// 从树上将自己裁剪下来
		/// </summary>
		public INode CutSelf();

		/// <summary>
		/// 从树上将自己裁剪下来时的处理
		/// </summary>
		/// <remarks>由框架内部调用</remarks>
		public void OnCutSelf();

		#endregion

		#region 释放

		/// <summary>
		/// 释放所有分支的所有节点
		/// </summary>
		public void RemoveAllNode();

		/// <summary>
		/// 释放分支的所有节点
		/// </summary>
		public void RemoveAllNode(long branchType);

		/// <summary>
		/// 释放前的处理
		/// </summary>
		/// <remarks>由框架内部调用</remarks>
		public void OnBeforeDispose();

		#endregion

		#endregion
	}
}