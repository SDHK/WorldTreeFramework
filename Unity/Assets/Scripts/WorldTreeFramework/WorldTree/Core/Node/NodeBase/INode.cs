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
	/// 世界树节点可视化生成器接口
	/// </summary>
	public interface IWorldTreeNodeViewBuilder : INode
	{ }


	/// <summary>
	/// 节点状态
	/// </summary>
	[Flags]
	public enum NodeState
	{
		/// <summary>
		/// 无状态
		/// </summary>
		None = 0,

		/// <summary>
		/// 是否来自对象池
		/// </summary>
		IsFromPool = 1,

		/// <summary>
		/// 是否释放
		/// </summary>
		IsDisposed = 1 << 1,
	}


	/// <summary>
	/// Node类型 受保护成员特性标记
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class ProtectedAttribute : Attribute { }

	/// <summary>
	/// 世界树数据节点接口
	/// </summary>
	public partial interface INodeData : INode
	{
		/// <summary>
		/// 数据id
		/// </summary>
		/// <remarks>雪花id: 雪花算法生成的唯一id</remarks>
		public long UID { get; set; }
	}


	/// <summary>
	/// 世界树节点接口
	/// </summary>
	/// <remarks>
	/// <para>世界树节点的最底层接口</para>
	/// <para>部分类型直接继承INode接口，将会生成对应的Copy Node内容的部分类</para>
	/// </remarks>
	[TreeDataSerializable]
	public partial interface INode : IWorldTreeBasic
		, AsRule<Enable>
		, AsRule<Add>
		, AsRule<Graft>
		, AsRule<Update>
		, AsRule<UpdateTime>
		, AsRule<Cut>
		, AsRule<BeforeRemove>
		, AsRule<Disable>
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
		[TreeDataIgnore]
		public WorldTreeRoot Root { get; set; }

		/// <summary>
		/// 树枝节点
		/// </summary>
		/// <remarks>用于划分作用域</remarks>
		[TreeDataIgnore]
		public INode Domain { get; set; }

		/// <summary>
		/// 父节点
		/// </summary>
		[TreeDataIgnore]
		public INode Parent { get; set; }

		/// <summary>
		/// 可视化生成器
		/// </summary>
		[TreeDataIgnore]
		public IWorldTreeNodeViewBuilder View { get; set; }

		#region Active

		/// <summary>
		/// 活跃开关
		/// </summary>
		[TreeDataIgnore]
		public bool ActiveToggle { get; set; }

		/// <summary>
		/// 活跃状态(设定为只读，禁止修改)
		/// </summary>
		public bool IsActive { get; set; }

		/// <summary>
		/// 活跃事件标记，这个由框架内部调用设置，禁止修改
		/// </summary>
		[TreeDataIgnore]
		public bool activeEventMark { get; set; }

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
		[TreeDataIgnore]
		public UnitDictionary<long, IRattan> RattanDict { get; set; }

		/// <summary>
		/// 树藤分支,假如没有则创建
		/// </summary>
		[TreeDataIgnore]
		public UnitDictionary<long, IRattan> GetRattanDict { get; }

		#endregion

		#region Branch

		/// <summary>
		/// 此节点挂载到父级的分支类型
		/// </summary>
		public long BranchType { get; }

		/// <summary>
		/// 树分支
		/// </summary>
		public UnitDictionary<long, IBranch> BranchDict { get; set; }

		/// <summary>
		/// 树分支,假如没有则创建
		/// </summary>
		[TreeDataIgnore]
		public UnitDictionary<long, IBranch> GetBranchDict { get; }

		#endregion

		#region 节点处理

		#region 添加

		/// <summary>
		/// 尝试添加到树结构上
		/// </summary>
		public bool TryAddSelfToTree<B, K>(K key, INode parent) where B : class, IBranch<K>;

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