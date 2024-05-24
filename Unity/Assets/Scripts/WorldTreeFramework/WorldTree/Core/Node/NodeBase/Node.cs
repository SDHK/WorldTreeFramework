/****************************************

* 创 建 者：  闪电黑客
* 创建时间：  2022/5/6 21:27
* 描    述:   世界树节点基类

****************************************/

namespace WorldTree
{
	/// <summary>
	/// 世界树节点基类
	/// </summary>
	public abstract partial class Node : INode
	{
		public bool IsFromPool { get; set; }
		public bool IsRecycle { get; set; }

		public bool IsDisposed { get; set; }

		public long Id { get; set; }
		public long Type { get; set; }

		public WorldTreeCore Core { get; set; }
		public WorldTreeRoot Root { get; set; }
		public INode Domain { get; set; }//接口标记域节点
		public INode Parent { get; set; }

		/// <summary>
		/// 调试可视化节点
		/// </summary>
		public IWorldTreeNodeViewBuilder View { get; set; }

		#region Active

		public bool ActiveToggle { get; set; }

		public bool IsActive { get; set; }

		public bool m_ActiveEventMark { get; set; }

		#endregion

		#region Rattan

		public UnitDictionary<long, IRattan> m_Rattans { get; set; }

		public UnitDictionary<long, IRattan> Rattans { get; }

		#endregion

		#region Branch

		public long BranchType { get; set; }

		public UnitDictionary<long, IBranch> m_Branchs { get; set; }

		public UnitDictionary<long, IBranch> Branchs => this.m_Branchs ??= this.PoolGetUnit<UnitDictionary<long, IBranch>>();

		#endregion

		public override string ToString()
		{
			return GetType().ToString();
		}

		#region 节点处理

		#region 添加

		public virtual bool TryAddSelfToTree<B, K>(K Key, INode parent)
			where B : class, IBranch<K>
		{
			if (parent.AddBranch<B>().TryAddNode(Key, this))
			{
				this.BranchType = TypeInfo<B>.TypeCode;
				this.Parent = parent;
				this.Core = parent.Core;
				this.Root = parent.Root;
				if (this.Domain != this) this.Domain = parent.Domain;
				this.SetActive(true);//激活节点
				this.View?.Dispose();
				this.View = this.Parent?.View != null ? Parent.View.Parent.AddChild<INode, INode>(Parent.View.Type, out _, this, Parent) as IWorldTreeNodeViewBuilder : null;
				return true;
			}
			return false;
		}

		public virtual void OnAddSelfToTree()
		{
			this.Core.ReferencedPoolManager.TryAdd(this);//添加到引用池
			if (this is not IListenerIgnorer)//广播给全部监听器
			{
				this.GetListenerActuator<IListenerAddRule>()?.Send((INode)this);
			}
			if (this is INodeListener nodeListener && this is not IListenerIgnorer)//检测添加静态监听
			{
				this.Core.ReferencedPoolManager.TryAddStaticListener(nodeListener);
			}
			if (this.IsActive != this.m_ActiveEventMark)//激活变更
			{
				if (this.IsActive)
				{
					this.Core.EnableRuleGroup?.Send(this);//激活事件通知
				}
				else
				{
					this.Core.DisableRuleGroup?.Send(this); //禁用事件通知
				}
			}
			this.Core.AddRuleGroup?.Send(this);//节点添加事件通知
		}

		#endregion

		#region 释放

		public virtual void RemoveAllNode()
		{
			if (this.m_Branchs == null) return;
			using (this.PoolGetUnit(out UnitStack<IBranch> branchs))
			{
				foreach (var item in this.m_Branchs) branchs.Push(item.Value);
				while (branchs.Count != 0) RemoveAllNode(branchs.Pop().Type);
			}

			//假如在分支移除过程中，节点又添加了新的分支。那么就是错误的，新增分支将无法回收。
			if (m_Branchs.Count != 0)
			{
				foreach (var item in m_Branchs)
				{
					this.Log($"移除分支出错，意外的新分支，节点：{this} 分支:{item.GetType()}");
				}
			}
		}

		public virtual void RemoveAllNode(long branchType)
		{
			if (this.TryGetBranch(branchType, out IBranch branch))
			{
				if (branch.Count != 0)
				{
					//迭代器是没法一边迭代一边删除的，所以这里用了一个栈来存储需要删除的节点
					using (this.PoolGetUnit(out UnitStack<INode> nodes))
					{
						foreach (var item in branch) nodes.Push(item);
						while (nodes.Count != 0) nodes.Pop().Dispose();
					}

					//假如在节点移除过程中，节点又添加了新的节点。那么就是错误的，新增节点将无法回收，父节点的分支键值将被占用。
					if (branch.Count != 0)
					{
						foreach (var item in branch)
						{
							this.LogError($"移除节点出错，意外的新节点，父级:{this.GetType()} 分支: {branch.GetType()} 节点:{item.GetType()}:{item.Id}");
						}
					}
				}
			}
		}

		/// <summary>
		/// 回收节点
		/// </summary>
		public virtual void Dispose()
		{
			//是否已经回收
			if (this.IsRecycle || this.IsDisposed) return;

			//节点回收前序遍历处理,节点回收后续遍历处理
			this.TraversalPrePostOrder(current => current.OnBeforeDispose(), current => current.OnDispose());
		}

		public virtual void OnBeforeDispose() => this.Core.BeforeRemoveRuleGroup?.Send(this);

		public virtual void OnDispose()
		{
			this.View?.Dispose();
			this.View = null;
			this.Parent.RemoveBranchNode(this.BranchType, this);//从父节点分支移除
			this.SetActive(false);//激活变更
			this.Core.DisableRuleGroup?.Send(this); //禁用事件通知
			if (this is INodeListener nodeListener && this is not IListenerIgnorer)
			{
				//检测移除静态监听
				this.Core.ReferencedPoolManager.RemoveStaticListener(nodeListener);
				if (nodeListener is IDynamicNodeListener dynamicNodeListener)
				{
					//检测移除动态监听
					this.Core.ReferencedPoolManager.RemoveDynamicListener(dynamicNodeListener);
				}
			}
			this.Core.RemoveRuleGroup?.Send(this);//移除事件通知
			if (this is not IListenerIgnorer)//广播给全部监听器通知 X
			{
				this.GetListenerActuator<IListenerRemoveRule>()?.Send((INode)this);
			}
			this.Core.ReferencedPoolManager.Remove(this);//引用池移除

			//this.DisposeDomain(); //清除域节点
			this.Parent = null;//清除父节点
			this.PoolRecycle(this);//回收到池
		}

		#endregion

		#region 嫁接

		public virtual bool TryGraftSelfToTree<B, K>(K key, INode parent)
			where B : class, IBranch<K>
		{
			if (!parent.AddBranch<B>().TryAddNode(key, this)) return false;

			this.BranchType = TypeInfo<B>.TypeCode;
			this.Parent = parent;
			this.Core = parent.Core;
			this.Root = parent.Root;
			if (this.Domain != this) this.Domain = parent.Domain;

			this.RefreshActive();
			this.TraversalLevel(current => current.OnGraftSelfToTree());
			return true;
		}

		public virtual void OnGraftSelfToTree()//id相同数据同步？
		{
			this.View?.Dispose();
			this.View = this.Parent?.View != null ? Parent.View.Parent.AddChild<INode, INode>(Parent.View.Type, out _, this, Parent) as IWorldTreeNodeViewBuilder : null;
			this.Core = this.Parent.Core;
			this.Root = this.Parent.Root;
			if (this.Domain != this) this.Domain = this.Parent.Domain;

			this.Core.ReferencedPoolManager.TryAdd(this);//添加到引用池
			if (this is not IListenerIgnorer)//广播给全部监听器
			{
				this.GetListenerActuator<IListenerAddRule>()?.Send((INode)this);
			}
			if (this is INodeListener nodeListener && this is not IListenerIgnorer)//检测添加静态监听
			{
				this.Core.ReferencedPoolManager.TryAddStaticListener(nodeListener);
			}
			if (this.IsActive != this.m_ActiveEventMark)//激活变更
			{
				if (this.IsActive)
				{
					this.Core.EnableRuleGroup?.Send(this);//激活事件通知
				}
				else
				{
					this.Core.DisableRuleGroup?.Send(this); //禁用事件通知
				}
			}
			NodeRuleHelper.SendRule(this, TypeInfo<Graft>.Default);
		}

		#endregion

		#region 裁剪

		public virtual INode CutSelf()
		{
			if (this.IsRecycle) return null; //是否已经回收
			this.TraversalPostorder(current => current.OnCutSelf());
			this.Parent.RemoveBranchNode(this.BranchType, this);//从父节点分支移除
			return this;
		}

		public virtual void OnCutSelf()
		{
			this.View?.Dispose();
			this.View = null;
			if (this is INodeListener nodeListener && this is not IListenerIgnorer)
			{
				//检测移除静态监听
				this.Core.ReferencedPoolManager.RemoveStaticListener(nodeListener);
				if (nodeListener is IDynamicNodeListener dynamicNodeListener)
				{
					//检测移除动态监听
					this.Core.ReferencedPoolManager.RemoveDynamicListener(dynamicNodeListener);
				}
			}
			NodeRuleHelper.SendRule(this, TypeInfo<Cut>.Default);
			if (this is not IListenerIgnorer)//广播给全部监听器通知 X
			{
				this.GetListenerActuator<IListenerRemoveRule>()?.Send((INode)this);
			}
			this.Core.ReferencedPoolManager.Remove(this);//引用池移除 ?
			this.Parent = null;//清除父节点
		}

		#endregion

		#endregion
	}
}