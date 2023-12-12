
/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/10 12:00

* 描述： 树字典

*/

using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 树字典泛型类
	/// </summary>
	public class TreeDictionary<Key, V> : Dictionary<Key, V>, INode, ComponentOf<INode>, ChildOf<INode>
		, AsRule<IAwakeRule>
	{
		public bool IsFromPool { get; set; }
		public bool IsRecycle { get; set; }
		public bool IsDisposed { get; set; }
		public long Id { get; set; }
		public long DataId { get; set; }
		public long Type { get; set; }
		public WorldTreeCore Core { get; set; }
		public WorldTreeRoot Root { get; set; }
		public INode Domain { get; set; }
		public INode Parent { get; set; }

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

		/// <summary>
		/// 分支类型
		/// </summary>
		public long BranchType { get; set; }

		/// <summary>
		/// 树分支
		/// </summary>
		public UnitDictionary<long, IBranch> m_Branchs { get; set; }
		public UnitDictionary<long, IBranch> Branchs => this.m_Branchs ??= this.PoolGet<UnitDictionary<long, IBranch>>();


		#endregion


		#region 节点处理

		#region 添加

		public virtual INode AddSelfToTree<B, K>(K key, INode parent)
			where B : class, IBranch<K>
		{
			if (parent.AddBranch<B>().TryAddNode(key, this))
			{
				this.BranchType = TypeInfo<B>.TypeCode;
				this.Parent = parent;
				this.Core = parent.Core;
				this.Root = parent.Root;
				if (this.Domain != this) this.Domain = parent.Domain;

				this.SetActive(true);//激活节点
				this.TrySendRule(TypeInfo<IAwakeRule>.Default);
				this.OnAddSelfToTree();
			}
			return this;
		}

		public virtual INode AddSelfToTree<B, K, T1>(K key, INode parent, T1 arg1)
			where B : class, IBranch<K>
		{
			if (parent.AddBranch<B>().TryAddNode(key, this))
			{
				this.BranchType = TypeInfo<B>.TypeCode;
				this.Parent = parent;
				this.Core = parent.Core;
				this.Root = parent.Root;
				if (this.Domain != this) this.Domain = parent.Domain;

				this.SetActive(true);//激活节点
				this.TrySendRule(TypeInfo<IAwakeRule<T1>>.Default, arg1);
				this.OnAddSelfToTree();
			}
			return this;
		}

		public virtual INode AddSelfToTree<B, K, T1, T2>(K key, INode parent, T1 arg1, T2 arg2)
			where B : class, IBranch<K>
		{
			if (parent.AddBranch<B>().TryAddNode(key, this))
			{
				this.BranchType = TypeInfo<B>.TypeCode;
				this.Parent = parent;
				this.Core = parent.Core;
				this.Root = parent.Root;
				if (this.Domain != this) this.Domain = parent.Domain;

				this.SetActive(true);//激活节点
				this.TrySendRule(TypeInfo<IAwakeRule<T1, T2>>.Default, arg1, arg2);
				this.OnAddSelfToTree();
			}
			return this;
		}

		public virtual INode AddSelfToTree<B, K, T1, T2, T3>(K key, INode parent, T1 arg1, T2 arg2, T3 arg3)
			where B : class, IBranch<K>
		{
			if (parent.AddBranch<B>().TryAddNode(key, this))
			{
				this.BranchType = TypeInfo<B>.TypeCode;
				this.Parent = parent;
				this.Core = parent.Core;
				this.Root = parent.Root;
				if (this.Domain != this) this.Domain = parent.Domain;

				this.SetActive(true);//激活节点
				this.TrySendRule(TypeInfo<IAwakeRule<T1, T2, T3>>.Default, arg1, arg2, arg3);
				this.OnAddSelfToTree();
			}
			return this;
		}

		public virtual INode AddSelfToTree<B, K, T1, T2, T3, T4>(K key, INode parent, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
			where B : class, IBranch<K>
		{
			if (parent.AddBranch<B>().TryAddNode(key, this))
			{
				this.BranchType = TypeInfo<B>.TypeCode;
				this.Parent = parent;
				this.Core = parent.Core;
				this.Root = parent.Root;
				if (this.Domain != this) this.Domain = parent.Domain;

				this.SetActive(true);//激活节点
				this.TrySendRule(TypeInfo<IAwakeRule<T1, T2, T3, T4>>.Default, arg1, arg2, arg3, arg4);
				this.OnAddSelfToTree();
			}
			return this;
		}

		public virtual INode AddSelfToTree<B, K, T1, T2, T3, T4, T5>(K key, INode parent, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
			where B : class, IBranch<K>
		{
			if (parent.AddBranch<B>().TryAddNode(key, this))
			{
				this.BranchType = TypeInfo<B>.TypeCode;
				this.Parent = parent;
				this.Core = parent.Core;
				this.Root = parent.Root;
				if (this.Domain != this) this.Domain = parent.Domain;

				this.SetActive(true);//激活节点
				this.TrySendRule(TypeInfo<IAwakeRule<T1, T2, T3, T4, T5>>.Default, arg1, arg2, arg3, arg4, arg5);
				this.OnAddSelfToTree();
			}
			return this;
		}

		public virtual void OnAddSelfToTree()
		{
			this.Core.ReferencedPoolManager.TryAdd(this);//添加到引用池
			if (this is not ICoreNode)//广播给全部监听器
			{
				this.GetListenerActuator<IListenerAddRule>()?.Send((INode)this);
			}
			if (this is INodeListener nodeListener && this is not ICoreNode)//检测添加静态监听
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
			using (this.PoolGet(out UnitStack<IBranch> branchs))
			{
				foreach (var item in this.m_Branchs) branchs.Push(item.Value);
				while (branchs.Count != 0) RemoveAllNode(branchs.Pop().Type);
			}
			//假如在分支移除过程中，节点又添加了新的分支。那么就是错误的，新增分支将无法回收。
			if (m_Branchs.Count != 0)
			{
				foreach (var item in m_Branchs)
				{
					World.Log($"移除分支出错，意外的新分支，节点：{this} 分支:{item.GetType()}");
				}
			}
		}

		public virtual void RemoveAllNode<B>() where B : class, IBranch => RemoveAllNode(TypeInfo<B>.TypeCode);

		public virtual void RemoveAllNode(long branchType)
		{
			if (this.TryGetBranch(branchType, out IBranch branch))
			{
				if (branch.Count != 0)
				{
					//迭代器是没法一边迭代一边删除的，所以这里用了一个栈来存储需要删除的节点
					using (Core.PoolGet(out UnitStack<INode> nodes))
					{
						foreach (var item in branch) nodes.Push(item);
						while (nodes.Count != 0) nodes.Pop().Dispose();
					}
					//假如在节点移除过程中，节点又添加了新的节点。那么就是错误的，新增节点将无法回收，父节点的分支键值将被占用。
					if (branch.Count != 0)
					{
						foreach (var item in branch)
						{
							World.LogError($"移除节点出错，意外的新节点，分支:{this.GetType()} 节点:{item.GetType()}:{item.Id}");
						}
					}
				}
			}
		}

		public virtual void RemoveNode<B, K>(K key) where B : class, IBranch<K> => this.GetBranch<B>()?.GetNode(key)?.Dispose();

		public virtual void RemoveNodeById<B>(long id) where B : class, IBranch => this.GetBranch<B>()?.GetNodeById(id)?.Dispose();

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

		public void OnBeforeDispose() => this.Core.BeforeRemoveRuleGroup?.Send(this);

		public virtual void OnDispose()//未完
		{
			this.Parent?.RemoveBranchNode(this.BranchType, this);//从父节点分支移除
			this.SendAllReferencedNodeRemove();//_判断移除引用关系 X
			this.SetActive(false);//激活变更
			this.Core.DisableRuleGroup?.Send(this); //禁用事件通知 X
			if (this is INodeListener nodeListener && this is not ICoreNode)
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
			if (this is not ICoreNode)//广播给全部监听器通知 X
			{
				this.GetListenerActuator<IListenerRemoveRule>()?.Send((INode)this);
			}
			this.Core.ReferencedPoolManager.Remove(this);//引用池移除 ?
														 //this.DisposeDomain(); //清除域节点
			this.Parent = null;//清除父节点
			Clear();
			Core?.Recycle(this);//回收到池
		}

		#endregion

		#region 嫁接

		public virtual bool GraftSelfToTree<B, K>(K key, INode parent)
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
			this.Core = this.Parent.Core;
			this.Root = this.Parent.Root;
			if (this.Domain != this) this.Domain = this.Parent.Domain;

			this.Core.ReferencedPoolManager.TryAdd(this);//添加到引用池
			if (this is not ICoreNode)//广播给全部监听器
			{
				this.GetListenerActuator<IListenerAddRule>()?.Send((INode)this);
			}
			if (this is INodeListener nodeListener && this is not ICoreNode)//检测添加静态监听
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
			this.SendRule(TypeInfo<IGraftRule>.Default);
		}

		#endregion

		#region 裁剪

		public virtual bool TryCutNodeById<B>(long id, out INode node) where B : class, IBranch => (node = this.GetBranch<B>()?.GetNodeById(id).CutSelf()) != null;
		public virtual bool TryCutNode<B, K>(K key, out INode node) where B : class, IBranch<K> => (node = this.GetBranch<B>()?.GetNode(key).CutSelf()) != null;

		public virtual INode CutNodeById<B>(long id) where B : class, IBranch => this.GetBranch<B>()?.GetNodeById(id).CutSelf();
		public virtual INode CutNode<B, K>(K key) where B : class, IBranch<K> => this.GetBranch<B>()?.GetNode(key).CutSelf();

		public virtual INode CutSelf()
		{
			if (this.IsRecycle) return null; //是否已经回收
			this.TraversalPostorder(current => current.OnCutSelf());
			this.Parent?.RemoveBranchNode(this.BranchType, this);//从父节点分支移除
			return this;
		}
		public virtual void OnCutSelf()
		{
			this.SendAllReferencedNodeRemove();//_判断移除引用关系 X
			if (this is INodeListener nodeListener && this is not ICoreNode)
			{
				//检测移除静态监听
				this.Core.ReferencedPoolManager.RemoveStaticListener(nodeListener);
				if (nodeListener is IDynamicNodeListener dynamicNodeListener)
				{
					//检测移除动态监听
					this.Core.ReferencedPoolManager.RemoveDynamicListener(dynamicNodeListener);
				}
			}
			this.SendRule(TypeInfo<ICutRule>.Default);
			if (this is not ICoreNode)//广播给全部监听器通知 X
			{
				this.GetListenerActuator<IListenerRemoveRule>()?.Send((INode)this);
			}
			this.Core.ReferencedPoolManager.Remove(this);//引用池移除 ?
		}

		#endregion

		#region 获取	

		public virtual bool ContainsId<B>(long id) where B : class, IBranch => this.GetBranch<B>()?.ContainsId(id) ?? false;

		public virtual bool Contains<B, K>(K key) where B : class, IBranch<K> => this.GetBranch<B>()?.Contains(key) ?? false;

		public virtual bool TryGetNodeById<B>(long id, out INode node) where B : class, IBranch => (node = this.TryGetBranch(out B branch) && branch.TryGetNodeById(id, out node) ? node : null) != null;

		public virtual bool TryGetNode<B, K>(K key, out INode node) where B : class, IBranch<K> => (node = this.TryGetBranch(out B branch) && branch.TryGetNode(key, out node) ? node : null) != null;
		public virtual INode GetNodeById<B>(long Id) where B : class, IBranch => this.GetBranch<B>()?.GetNodeById(Id);
		public virtual INode GetNode<B, K>(K key) where B : class, IBranch<K> => this.GetBranch<B>()?.GetNode(key);

		#endregion

		#endregion

	}

}
