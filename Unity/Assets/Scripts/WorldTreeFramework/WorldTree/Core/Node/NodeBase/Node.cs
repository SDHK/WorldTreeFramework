/****************************************

* 创 建 者：  闪电黑客
* 创建时间：  2022/5/6 21:27
* 描    述:   世界树节点基类

****************************************/

namespace WorldTree
{
	/// <summary>
	/// 世界树数据节点基类，ID由雪花算法生成
	/// </summary>
	public abstract class NodeData : Node, INodeData
	{
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

		#region Active


		public bool ActiveToggle { get; set; }

		[TreeDataIgnore]
		public bool IsActive { get; set; }

		[TreeDataIgnore]
		public bool activeEventMark { get; set; }

		public void SetActive(bool value)
		{
			if (ActiveToggle != value)
			{
				ActiveToggle = value;
				RefreshActive();
			}
		}

		public void RefreshActive()
		{
			//如果状态相同，不需要刷新
			if (IsActive == ((Parent == null) ? ActiveToggle : Parent.IsActive && ActiveToggle)) return;

			//层序遍历设置子节点
			using (Core.PoolGetUnit(out UnitQueue<INode> queue))
			{
				queue.Enqueue(this);
				while (queue.Count != 0)
				{
					// 广度优先，出队
					var current = queue.Dequeue();
					if (current.IsActive != ((current.Parent == null) ? current.ActiveToggle : current.Parent.IsActive && current.ActiveToggle))
					{
						current.IsActive = !current.IsActive;

						if (current.BranchDict != null)
						{
							foreach (var branchs in current.BranchDict)
							{
								foreach (INode node in branchs.Value)
								{
									if (node.BranchType == branchs.Value.Type)
									{
										queue.Enqueue(node);
									}
								}
							}
						}
					}
				}
			}
		}

		#endregion

		#region Rattan

		[TreeDataIgnore]
		public UnitDictionary<long, IRattan> RattanDict { get; set; }

		[TreeDataIgnore]
		public UnitDictionary<long, IRattan> GetRattanDict { get; }

		#endregion

		#region Branch

		/// <summary>
		/// 此节点挂载到父级的分支类型
		/// </summary>
		[TreeDataIgnore]
		public long BranchType { get; set; }


		public BranchGroup BranchDict { get; set; }

		[TreeDataIgnore]
		public BranchGroup GetBranchDict => BranchDict ??= Core.PoolGetUnit<BranchGroup>();

		#endregion



		public override string ToString()
		{
			return GetType().ToString();
		}

		#region 节点处理

		#region 创建

		/// <summary>
		/// 创建时：Node的Id获取和法则支持
		/// </summary>
		public virtual void OnCreate()
		{
			InstanceId = Core.IdManager.GetId();
			Id = InstanceId;
			Core.RuleManager?.SupportNodeRule(Type);
		}

		#endregion

		#region 添加

		public virtual bool TryAddSelfToTree<B, K>(K key, INode parent)
			where B : class, IBranch<K>
		{
			if (NodeBranchHelper.AddBranch<B>(parent).TryAddNode(key, this))
			{
				BranchType = Core.TypeToCode<B>();
				Parent = parent;
				Core = parent.Core;
				World = parent.World;
				SetActive(true);//激活节点
				AddNodeView();
				return true;
			}
			return false;
		}

		public virtual void OnAddSelfToTree()
		{
			Core.ReferencedPoolManager.TryAdd(this);//添加到引用池
			if (this is not IListenerIgnorer)//广播给全部监听器
			{
				IRuleExecutor<IListenerAddRule> ruleActuator = NodeListenerExecutorHelper.GetListenerExecutor<IListenerAddRule>(this);
				ruleActuator?.Send((INode)this);
			}
			if (this is INodeListener nodeListener && this is not IListenerIgnorer)//检测自身是否为监听器
			{
				Core.ReferencedPoolManager.TryAddListener(nodeListener);
			}
			if (IsActive != activeEventMark)//激活变更
			{
				if (IsActive)
				{
					Core.EnableRuleGroup?.Send(this);//激活事件通知
				}
				else
				{
					Core.DisableRuleGroup?.Send(this); //禁用事件通知
				}
			}
			Core.AddRuleGroup?.Send(this);//节点添加事件通知
		}

		#endregion

		#region 移除

		public virtual void RemoveAllNode()
		{
			if (BranchDict == null) return;
			using (Core.PoolGetUnit(out UnitStack<IBranch> branchs))
			{
				foreach (var item in BranchDict) branchs.Push(item.Value);
				while (branchs.Count != 0) RemoveAllNode(branchs.Pop().Type);
			}

			//假如在分支移除过程中，节点又添加了新的分支。那么就是错误的，新增分支将无法回收。
			if (BranchDict.Count != 0)
			{
				foreach (var item in BranchDict)
				{
					this.Log($"移除分支出错，意外的新分支，节点：{this} 分支:{item.GetType()}");
				}
			}
		}

		public virtual void RemoveAllNode(long branchType)
		{
			if (NodeBranchHelper.TryGetBranch(this, branchType, out IBranch branch))
			{
				if (branch.Count != 0)
				{
					//迭代器是没法一边迭代一边删除的，所以这里用了一个栈来存储需要删除的节点
					using (Core.PoolGetUnit(out UnitStack<INode> nodes))
					{
						foreach (var item in branch) nodes.Push(item);
						while (nodes.Count != 0) nodes.Pop().Dispose();
					}

					//假如在节点移除过程中，节点又添加了新的节点。那么就是错误的，新增节点将无法回收，父节点的分支键值将被占用。
					if (branch.Count != 0)
					{
						foreach (var item in branch)
						{
							this.LogError($"移除节点出错，意外的新节点，父级:{GetType()} 分支: {branch.GetType()} 节点:{item.GetType()}:{item.Id}");
						}
					}
				}
			}
		}

		/// <summary>
		/// 释放节点
		/// </summary>
		public virtual void Dispose()
		{
			//是否已经释放
			if (IsDisposed) return;

			//节点释放前序遍历处理,节点释放后续遍历处理
			NodeBranchTraversalHelper.TraversalPrePostOrder(this, current => current.OnBeforeDispose(), current => current.OnDispose());
		}

		public virtual void OnBeforeDispose() => Core.BeforeRemoveRuleGroup?.Send(this);

		public virtual void OnDispose()
		{
			ViewBuilder?.Core.WorldContext.Post(ViewBuilderDispose);
			NodeBranchHelper.RemoveNode(this);//从父节点分支移除
			SetActive(false);//激活变更
			Core.DisableRuleGroup?.Send(this); //禁用事件通知
			if (this is INodeListener nodeListener && this is not IListenerIgnorer)//检测自身为监听器
			{
				Core.ReferencedPoolManager.RemoveListener(nodeListener);
			}
			Core.RemoveRuleGroup?.Send(this);//移除事件通知
			if (this is not IListenerIgnorer)//广播给全部监听器通知
			{
				NodeListenerExecutorHelper.GetListenerExecutor<IListenerRemoveRule>(this)?.Send((INode)this);
			}
			Core.ReferencedPoolManager.Remove(this);//引用池移除

			//this.DisposeDomain(); //清除域节点
			Parent = null;//清除父节点
			Core.PoolRecycle(this);//回收到池
		}

		#endregion

		#region 嫁接

		public virtual bool TryGraftSelfToTree<B, K>(K key, INode parent)
			where B : class, IBranch<K>
		=> TryGraftSelfToTree(this.TypeToCode<B>(), key, parent);

		public virtual bool TryGraftSelfToTree<K>(long branchType, K key, INode parent)
		{
			if (NodeBranchHelper.AddBranch(parent, branchType) is not IBranch<K> branch) return false;
			if (!branch.TryAddNode(key, this)) return false;

			BranchType = branch.Type;
			Parent = parent;
			Core = parent.Core;
			World = parent.World;

			RefreshActive();
			NodeBranchTraversalHelper.TraversalPrePostOrder(this, current => current.OnBeforeGraftSelfToTree(), current => current.OnGraftSelfToTree());
			return true;
		}

		public virtual void OnBeforeGraftSelfToTree()
		{
			this.Core = this.Parent.Core;
			this.World = this.Parent.World;
			//序列化时，需要重新设置所有节点的父节点
			if (IsSerialize)
			{
				if (BranchDict != null)
				{
					foreach (var brancItem in BranchDict)
					{
						if (brancItem.Value == null) continue;
						foreach (var nodeItem in brancItem.Value)
						{
							nodeItem.Parent = this;
							nodeItem.BranchType = brancItem.Value.Type;
						}
					}
				}
			}
			AddNodeView();
		}

		public virtual void OnGraftSelfToTree()
		{
			Core.ReferencedPoolManager.TryAdd(this);//添加到引用池
			if (this is not IListenerIgnorer)//广播给全部监听器
			{
				IRuleExecutor<IListenerAddRule> ruleActuator = NodeListenerExecutorHelper.GetListenerExecutor<IListenerAddRule>(this);
				ruleActuator?.Send((INode)this);
			}
			if (this is INodeListener nodeListener && this is not IListenerIgnorer)//检测添加静态监听
			{
				Core.ReferencedPoolManager.TryAddListener(nodeListener);
			}

			if (IsSerialize)
			{
				Core.DeserializeRuleGroup?.Send(this);//反序列化事件通知
				IsSerialize = false;
			}

			if (IsActive != activeEventMark)//激活变更
			{
				if (IsActive)
				{
					Core.EnableRuleGroup?.Send(this);//激活事件通知
				}
				else
				{
					Core.DisableRuleGroup?.Send(this); //禁用事件通知
				}
			}
			if (!IsSerialize) Core.GraftRuleGroup?.Send(this);//嫁接事件通知
		}

		#endregion

		#region 裁剪

		public virtual INode CutSelf()
		{
			if (IsDisposed) return null; //是否已经回收
			if (Parent == null) return this;
			NodeBranchTraversalHelper.TraversalPostorder(this, current => current.OnCutSelf());
			NodeBranchHelper.RemoveNode(this);//从父节点分支移除
			return this;
		}

		public virtual void OnCutSelf()
		{
			ViewBuilder?.Dispose();
			ViewBuilder = null;
			if (this is INodeListener nodeListener && this is not IListenerIgnorer)
			{
				//检测移除静态监听
				Core.ReferencedPoolManager.RemoveListener(nodeListener);
			}
			Core.CutRuleGroup?.Send(this);//裁剪事件通知
			if (this is not IListenerIgnorer)//广播给全部监听器通知 X
			{
				NodeListenerExecutorHelper.GetListenerExecutor<IListenerRemoveRule>(this)?.Send((INode)this);
			}
			Core.ReferencedPoolManager.Remove(this);//引用池移除 ?
			Parent = null;//清除父节点
		}

		#endregion

		#endregion

		/// <summary>
		/// 添加节点可视化
		/// </summary>
		protected void AddNodeView()
		{
			ViewBuilder?.Core.WorldContext.Post(ViewBuilderDispose);
			Core.ViewBuilder?.Core.WorldContext.Post(ViewBuilderCreate);
		}

		/// <summary>
		/// 创建节点可视化
		/// </summary>
		private void ViewBuilderCreate()
		{
			if (this.Parent?.ViewBuilder == null)
			{
				ViewBuilder = null;
				return;
			}
			// 拿到父节点的可视化生成器的父级节点
			INode viewParent = Parent.ViewBuilder.Parent;

			// 生成自身的可视化生成器
			INode nodeView = viewParent.Core.PoolGetNode(Parent.ViewBuilder.Type);

			// 将自身添加到父节点的可视化生成器中，而可视化则挂到可视化父级节点上
			ViewBuilder = NodeBranchHelper.AddNodeToTree(viewParent, default(ChildBranch), nodeView.Id, nodeView, (INode)this, (INode)Parent) as IWorldTreeNodeViewBuilder;
		}

		/// <summary>
		/// 释放节点可视化
		/// </summary>
		private void ViewBuilderDispose()
		{
			ViewBuilder.Dispose();
			ViewBuilder = null;
		}
	}
}