using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 分支结构限制
	/// </summary>
	/// <typeparam name="P">父节点类型</typeparam>
	/// <typeparam name="B">分支类型</typeparam>
	public interface BranchOf<in P, in B> where P : class, INode where B : class, IBranch { }

	/// <summary>
	/// 世界树分支接口
	/// </summary>
	/// <remarks>
	/// <para>世界树节点的结构组织接口基类</para> 
	/// </remarks>
	public interface IBranch : IUnitPoolEventItem, IEnumerable<INode>
	{
		/// <summary>
		/// 自身节点
		/// </summary>
		public INode Self { get; set; }
		/// <summary>
		/// 子节点数量
		/// </summary>
		public int Count { get; }
		/// <summary>
		/// 移除节点
		/// </summary>
		public void RemoveNode(INode node);
		/// <summary>
		/// 移除所有节点
		/// </summary>
		public void RemoveAllNode();

	}


	//实例化
	//添加到引用池
	//广播给全部监听器
	//节点添加实例化通知

	//序列化
	//序列化通知
	//广播给全部监听器通知
	//引用池移除




}
