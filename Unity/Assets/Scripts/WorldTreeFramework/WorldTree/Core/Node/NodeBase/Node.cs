/****************************************

* 创 建 者：  闪电黑客
* 创建时间：  2022/5/6 21:27
* 描    述:   世界树节点基类

****************************************/

using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 测试法则
	/// </summary>
	public interface TestNodeEvent<X> : ISendRule<TestEnum, X, List<int>>
	{
	}
	/// <summary>
	/// 测试法则2
	/// </summary>
	public interface TestNodeEvent2<X> : ISendRule<TestEnum, X, List<int>>
	{
	}


	/// <summary>
	/// 测试枚举
	/// </summary>
	public enum TestEnum
	{
		/// <summary>
		/// 测试1
		/// </summary>
		Test1,
		/// <summary>
		/// 测试2
		/// </summary>
		Test2,
	}

	/// <summary>
	/// 世界树数据节点基类，ID由雪花算法生成
	/// </summary>
	public abstract class NodeData : Node, INodeData
	{
		public override long Id { get; set; }
		/// <summary>
		/// 创建时：NodeData的UID获取和法则支持
		/// </summary>
		override public void OnCreate()
		{
			//如果是新创建的节点，需要获取一个新的UID
			//如果是反序列化的节点，不需要获取新的UID
			if (!IsSerialize) Id = Core.IdManager.GetUID();
			InstanceId = Core.IdManager.GetId();
			Core.RuleManager.SupportNodeRule(Type);
		}
	}

	/// <summary>
	/// 世界树节点基类
	/// </summary>
	public abstract class Node : INode
	{
		/// <summary>
		/// 节点复数状态
		/// </summary>
		[TreeDataIgnore]
		protected NodeState state;

		[TreeDataIgnore]
		public bool IsFromPool
		{
			get => (state & NodeState.IsFromPool) == NodeState.IsFromPool;
			set
			{
				if (value)
				{
					state |= NodeState.IsFromPool;
				}
				else
				{
					state &= ~NodeState.IsFromPool;
				}
			}
		}
		[TreeDataIgnore]
		public bool IsDisposed
		{
			get => (state & NodeState.IsDisposed) == NodeState.IsDisposed;
			set
			{
				if (value)
				{
					state |= NodeState.IsDisposed;
				}
				else
				{
					state &= ~NodeState.IsDisposed;
				}
			}
		}
		[TreeDataIgnore]
		public bool IsSerialize
		{
			get => (state & NodeState.IsSerialize) == NodeState.IsSerialize;
			set
			{
				if (value)
				{
					state |= NodeState.IsSerialize;
				}
				else
				{
					state &= ~NodeState.IsSerialize;
				}
			}
		}

		[TreeDataIgnore]
		public virtual long Id { get; set; }

		[TreeDataIgnore]
		public long InstanceId { get; set; }

		[TreeDataIgnore]
		public long Type { get; set; }

		[TreeDataIgnore]
		public WorldLine Core { get; set; }

		[TreeDataIgnore]
		public World World { get; set; }

		[TreeDataIgnore]
		public INode Parent { get; set; }

		[TreeDataIgnore]
		public IWorldTreeNodeViewBuilder ViewBuilder { get; set; }

		#region Rattan

		[TreeDataIgnore]
		public UnitDictionary<long, IRattan> RattanDict { get; set; }

		#endregion

		#region Branch

		/// <summary>
		/// 此节点挂载到父级的分支类型
		/// </summary>
		[TreeDataIgnore]
		public long BranchType { get; set; }

		public IBranchBase BranchDict { get; set; }


		#endregion

		#region Active

		public bool ActiveToggle { get; set; }

		[TreeDataIgnore]
		public bool IsActive { get; set; }

		[TreeDataIgnore]
		public bool activeEventMark { get; set; }

		public void SetActive(bool value) => INodeProxyRule.SetActive(this, value);

		public void RefreshActive() => INodeProxyRule.RefreshActive(this);

		#endregion

		public override string ToString() => INodeProxyRule.ToString(this);

		#region 节点处理

		#region 创建

		public virtual void OnCreate() => INodeProxyRule.OnCreate(this);

		#endregion

		#region 添加

		public virtual bool TryAddSelfToTree<B, K>(K key, INode parent) where B : class, IBranch<K> => INodeProxyRule.TryAddSelfToTree<B, K>(this, key, parent);

		public virtual void OnAddSelfToTree() => INodeProxyRule.OnAddSelfToTree(this);

		#endregion

		#region 移除

		public virtual void RemoveAllNode() => INodeProxyRule.RemoveAllNode(this);

		public virtual void RemoveAllNode(long branchType) => INodeProxyRule.RemoveAllNode(this, branchType);

		/// <summary>
		/// 释放节点
		/// </summary>
		public virtual void Dispose() => INodeProxyRule.Dispose(this);

		public virtual void OnBeforeDispose() => INodeProxyRule.OnBeforeDispose(this);

		public virtual void OnDispose() => INodeProxyRule.OnDispose(this);

		#endregion

		#region 嫁接

		public virtual bool TryGraftSelfToTree<B, K>(K key, INode parent) where B : class, IBranch<K> => INodeProxyRule.TryGraftSelfToTree<B, K>(this, key, parent);

		public virtual bool TryGraftSelfToTree<K>(long branchType, K key, INode parent) => INodeProxyRule.TryGraftSelfToTree(this, branchType, key, parent);

		public virtual void OnBeforeGraftSelfToTree() => INodeProxyRule.OnBeforeGraftSelfToTree(this);

		public virtual void OnGraftSelfToTree() => INodeProxyRule.OnGraftSelfToTree(this);

		#endregion

		#endregion

		#region 裁剪

		public virtual INode CutSelf() => INodeProxyRule.CutSelf(this);

		public virtual void OnCutSelf() => INodeProxyRule.OnCutSelf(this);

		#endregion
	}
}