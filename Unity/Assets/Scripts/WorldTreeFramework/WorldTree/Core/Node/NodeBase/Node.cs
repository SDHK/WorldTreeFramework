/****************************************

* 创 建 者：  闪电黑客
* 创建时间：  2022/5/6 21:27
* 描    述:   世界树节点基类

****************************************/

namespace WorldTree
{
	/// <summary>
	/// 世界树数据节点基类
	/// </summary>
	public abstract class NodeData : Node, INodeData
	{
		public long UID { get; set; }

		/// <summary>
		/// 创建时：NodeData的UID获取和法则支持
		/// </summary>
		override public void OnCreate()
		{
			UID = Core.IdManager.GetUID();
			Id = Core.IdManager.GetId();
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
		[TreePackIgnore]
		protected NodeState state;

		[TreeDataIgnore]
		[TreePackIgnore]
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
		[TreePackIgnore]
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
		[TreePackIgnore]
		public long Id { get; set; }

		[TreeDataIgnore]
		[TreePackIgnore]
		public long Type { get; set; }

		[TreeDataIgnore]
		[TreePackIgnore]
		public WorldTreeCore Core { get; set; }

		[TreeDataIgnore]
		[TreePackIgnore]
		public WorldTreeRoot Root { get; set; }

		[TreeDataIgnore]
		[TreePackIgnore]
		public INode Domain { get; set; }//接口标记域节点

		[TreeDataIgnore]
		[TreePackIgnore]
		public INode Parent { get; set; }

		[TreeDataIgnore]
		[TreePackIgnore]
		public IWorldTreeNodeViewBuilder View { get; set; }

		#region Active

		[TreeDataIgnore]
		[TreePackIgnore]
		public bool ActiveToggle { get; set; }

		public bool IsActive { get; set; }

		[TreeDataIgnore]
		[TreePackIgnore]
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
		[TreePackIgnore]
		public UnitDictionary<long, IRattan> RattanDict { get; set; }

		[TreeDataIgnore]
		[TreePackIgnore]
		public UnitDictionary<long, IRattan> GetRattanDict { get; }

		#endregion

		#region Branch

		/// <summary>
		/// 此节点挂载到父级的分支类型
		/// </summary>
		[TreeDataIgnore]
		[TreePackIgnore]
		protected long branchType;

		[TreeDataIgnore]
		[TreePackIgnore]
		public long BranchType => branchType;

		public UnitDictionary<long, IBranch> BranchDict { get; set; }

		[TreeDataIgnore]
		[TreePackIgnore]
		public UnitDictionary<long, IBranch> GetBranchDict => BranchDict ??= Core.PoolGetUnit<UnitDictionary<long, IBranch>>();

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
			Id = Core.IdManager.GetId();
			if (this is INodeData nodeData) nodeData.UID = Core.IdManager.GetUID();
			Core.RuleManager?.SupportNodeRule(Type);
		}

		#endregion

		#region 添加

		public virtual bool TryAddSelfToTree<B, K>(K key, INode parent)
			where B : class, IBranch<K>
		{
			if (NodeBranchHelper.AddBranch<B>(parent).TryAddNode(key, this))
			{
				branchType = Core.TypeToCode<B>();
				Parent = parent;
				Core = parent.Core;
				Root = parent.Root;
				if (Domain != this) Domain = parent.Domain;
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
				NodeListenerActuatorHelper.GetListenerActuator<IListenerAddRule>(this)?.Send((INode)this);
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
			View?.Dispose();
			View = null;
			NodeBranchHelper.RemoveBranchNode(Parent, BranchType, this);//从父节点分支移除
			SetActive(false);//激活变更
			Core.DisableRuleGroup?.Send(this); //禁用事件通知
			if (this is INodeListener nodeListener && this is not IListenerIgnorer)//检测自身为监听器
			{
				Core.ReferencedPoolManager.RemoveListener(nodeListener);
			}
			Core.RemoveRuleGroup?.Send(this);//移除事件通知
			if (this is not IListenerIgnorer)//广播给全部监听器通知
			{
				NodeListenerActuatorHelper.GetListenerActuator<IListenerRemoveRule>(this)?.Send((INode)this);
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
		{
			if (!NodeBranchHelper.AddBranch<B>(parent).TryAddNode(key, this)) return false;

			branchType = Core.TypeToCode<B>();
			Parent = parent;
			Core = parent.Core;
			Root = parent.Root;
			if (Domain != this) Domain = parent.Domain;

			RefreshActive();
			NodeBranchTraversalHelper.TraversalLevel(this, current => current.OnGraftSelfToTree());
			return true;
		}

		public virtual void OnGraftSelfToTree()//id相同数据同步？
		{
			AddNodeView();
			Core = Parent.Core;
			Root = Parent.Root;
			if (Domain != this) Domain = Parent.Domain;

			Core.ReferencedPoolManager.TryAdd(this);//添加到引用池
			if (this is not IListenerIgnorer)//广播给全部监听器
			{
				NodeListenerActuatorHelper.GetListenerActuator<IListenerAddRule>(this)?.Send((INode)this);
			}
			if (this is INodeListener nodeListener && this is not IListenerIgnorer)//检测添加静态监听
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
			Core.GraftRuleGroup?.Send(this);//嫁接事件通知
		}

		#endregion

		#region 裁剪

		public virtual INode CutSelf()
		{
			if (IsDisposed) return null; //是否已经回收
			NodeBranchTraversalHelper.TraversalPostorder(this, current => current.OnCutSelf());
			NodeBranchHelper.RemoveBranchNode(Parent, BranchType, this);//从父节点分支移除
			return this;
		}

		public virtual void OnCutSelf()
		{
			View?.Dispose();
			View = null;
			if (this is INodeListener nodeListener && this is not IListenerIgnorer)
			{
				//检测移除静态监听
				Core.ReferencedPoolManager.RemoveListener(nodeListener);
			}
			Core.CutRuleGroup?.Send(this);//裁剪事件通知
			if (this is not IListenerIgnorer)//广播给全部监听器通知 X
			{
				NodeListenerActuatorHelper.GetListenerActuator<IListenerRemoveRule>(this)?.Send((INode)this);
			}
			Core.ReferencedPoolManager.Remove(this);//引用池移除 ?
			Parent = null;//清除父节点
		}

		#endregion

		#region 序列化	

		/// <summary>
		/// 序列化
		/// </summary>
		/// <returns></returns>
		public virtual INode Serialize()
		{
			if (IsDisposed) return null; //是否已经回收
			NodeBranchTraversalHelper.TraversalPostorder(this, current => current.OnCutSelf());

			return this;
		}

		/// <summary>
		/// 序列化
		/// </summary>
		public virtual void OnSerializeSelfToTree()
		{
			View?.Dispose();
			View = null;
			if (this is INodeListener nodeListener && this is not IListenerIgnorer)
			{
				//检测移除静态监听
				Core.ReferencedPoolManager.RemoveListener(nodeListener);
			}
			Core.CutRuleGroup?.Send(this);//裁剪事件通知
			if (this is not IListenerIgnorer)//广播给全部监听器通知 X
			{
				NodeListenerActuatorHelper.GetListenerActuator<IListenerRemoveRule>(this)?.Send((INode)this);
			}
		}

		#endregion

		#region 反序列化	


		/// <summary>
		/// 序列化
		/// </summary>
		/// <typeparam name="B"></typeparam>
		/// <typeparam name="K"></typeparam>
		/// <param name="key"></param>
		/// <param name="parent"></param>
		/// <returns></returns>
		public bool Deserialize<B, K>(K key, INode parent)
			where B : class, IBranch<K>
		{
			if (!NodeBranchHelper.AddBranch<B>(parent).TryAddNode(key, this)) return false;

			branchType = Core.TypeToCode<B>();
			Parent = parent;
			Core = parent.Core;
			Root = parent.Root;
			if (Domain != this) Domain = parent.Domain;

			//NodeBranchTraversalHelper.TraversalLevel(this, current => current.OnGraftSelfToTree());
			RefreshActive();
			return true;
		}

		/// <summary>
		/// 序列化
		/// </summary>
		public virtual void OnDeserializeSelfToTree()//id相同数据同步？
		{
			AddNodeView();
			Core = Parent.Core;
			Root = Parent.Root;
			if (Domain != this) Domain = Parent.Domain;

			Core.ReferencedPoolManager.TryAdd(this);//添加到引用池
			if (this is not IListenerIgnorer)//广播给全部监听器
			{
				NodeListenerActuatorHelper.GetListenerActuator<IListenerAddRule>(this)?.Send((INode)this);
			}
			if (this is INodeListener nodeListener && this is not IListenerIgnorer)//检测添加静态监听
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
			Core.GraftRuleGroup?.Send(this);//嫁接事件通知
		}

		#endregion

		#endregion

		/// <summary>
		/// 添加节点可视化
		/// </summary>
		protected void AddNodeView()
		{
			View?.Dispose();
			if (Parent?.View != null)
			{
				// 拿到父节点的可视化生成器的父级节点
				INode viewParent = Parent.View.Parent;

				// 生成自身的可视化生成器
				INode nodeView = viewParent.Core.PoolGetNode(Parent.View.Type);

				// 将自身添加到父节点的可视化生成器中，而可视化则挂到可视化父级节点上
				View = NodeBranchHelper.AddSelfToTree<ChildBranch, long, INode, INode>(nodeView, nodeView.Id, viewParent, this, Parent) as IWorldTreeNodeViewBuilder;
			}
			else
			{
				View = null;
			}
		}
	}
}