/****************************************

* 创 建 者：  闪电黑客
* 创建时间：  2022/5/6 21:27
* 描    述:   世界树节点基类

****************************************/

using Codice.Client.Common.TreeGrouper;
using System;

namespace WorldTree
{
	/// <summary>
	/// 世界树核心节点基类
	/// </summary>
	public abstract partial class CoreNode : Node, ICoreNode { }

	/// <summary>
	/// 世界树节点基类
	/// </summary>
	public abstract partial class Node : INode//真的需要每一个节点都有id吗？
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

		#region Children

		public UnitDictionary<long, INode> m_Children { get; set; }
		#endregion


		#region Component
		public bool isComponent { get; set; }
		public UnitDictionary<long, INode> m_Components { get; set; }
		#endregion

		#region Domains


		public UnitDictionary<Type, INode> m_Domains { get; set; }
		#endregion

		#region Referenceds

		public UnitDictionary<long, INode> m_ReferencedParents { get; set; }

		public UnitDictionary<long, INode> m_ReferencedChilden { get; set; }

		#endregion

		#region Branch 

		public long BranchType { get; set; }

		public UnitDictionary<long, IBranch> m_Branchs { get; set; }

		public UnitDictionary<long, IBranch> Branchs => this.m_Branchs ??= this.PoolGet<UnitDictionary<long, IBranch>>();

		#endregion

		public override string ToString()
		{
			return GetType().ToString();
		}

		#region 分支处理

		#region 添加

		public virtual B AddBranch<B>()
			where B : class, IBranch
		{
			var Branchs = this.Branchs;
			if (!Branchs.TryGetValue(TypeInfo<B>.HashCode64, out IBranch iBranch))
			{
				Branchs.Add(TypeInfo<B>.HashCode64, iBranch = this.PoolGet<B>());
				iBranch.SetNode(this);
			}
			return iBranch as B;
		}

		public virtual IBranch AddBranch(long Type)
		{
			var Branchs = this.Branchs;
			if (!Branchs.TryGetValue(Type, out IBranch iBranch))
			{
				Branchs.Add(Type, iBranch = this.Core.GetUnit(Type) as IBranch);
				iBranch.SetNode(this);
			}
			return iBranch;
		}

		#endregion

		#region 移除 

		public virtual void RemoveBranch<B>() where B : class, IBranch => this.RemoveBranch(TypeInfo<B>.HashCode64);

		public virtual void RemoveBranch(long branchType)
		{
			if (this.m_Branchs != null && this.m_Branchs.TryGetValue(branchType, out IBranch iBranch) && iBranch.Count != 0)
			{
				//迭代器是没法一边迭代一边删除的，所以这里用了一个栈来存储需要删除的节点
				using (this.PoolGet(out UnitStack<INode> nodes))
				{
					foreach (var item in iBranch) nodes.Push(item);
					while (nodes.Count != 0) nodes.Pop().Dispose();
				}

				//假如在节点移除过程中，节点又添加了新的节点。那么就是错误的，新增节点将无法回收，父节点的分支键值将被占用。
				if (iBranch.Count != 0)
				{
					foreach (var item in iBranch)
					{
						World.Log($"移除分支出错，意外的新节点，分支:{iBranch.GetType()} 节点:{item.GetType()}:{item.Id}");
					}
				}
			}
		}

		public virtual void RemoveInParentBranch()
		{
			if (this.Parent.TryGetBranch(this.BranchType, out IBranch branch))
			{
				branch.RemoveNode(this);
				if (branch.Count == 0)
				{
					branch.Dispose();
					this.Parent.m_Branchs.Remove(this.BranchType);
					if (this.Parent.m_Branchs.Count == 0)
					{
						this.Parent.m_Branchs.Dispose();
						this.Parent.m_Branchs = null;
					}
				}
			}
		}

		#endregion

		#region 获取

		public virtual bool TryGetBranch<B>(out B branch) where B : class, IBranch
		{
			if (this.m_Branchs != null && this.m_Branchs.TryGetValue(TypeInfo<B>.HashCode64, out IBranch iBranch))
			{
				branch = iBranch as B;
				return true;
			}
			branch = null;
			return false;
		}

		public virtual bool TryGetBranch(long branchType, out IBranch branch)
		{
			if (this.m_Branchs != null && this.m_Branchs.TryGetValue(branchType, out IBranch iBranch))
			{
				branch = iBranch;
				return true;
			}
			branch = null;
			return false;
		}

		#endregion


		#endregion

		#region 节点处理

		#region 添加

		public virtual N TreeAddNode<B, K, N>(K key, out N node, bool isPool = true)
			where N : class, INode
			where B : class, IBranch<K>
		{
			if (this.AddBranch<B>().TryAddNode(key, out node, isPool))
			{
				node.SetActive(true);//激活节点
				node.TrySendRule(IAwakeRule.Default);
				node.OnTreeAddSelf();
			}
			return node;
		}

		public virtual N TreeAddNode<B, K, N, T1>(K key, out N node, T1 arg1, bool isPool = true)
			where N : class, INode
			where B : class, IBranch<K>
		{
			if (this.AddBranch<B>().TryAddNode(key, out node, isPool))
			{
				node.SetActive(true);//激活节点
				node.TrySendRule(IAwakeRule<T1>.Default, arg1);
				node.OnTreeAddSelf();
			}
			return node;
		}

		public virtual N TreeAddNode<B, K, N, T1, T2>(K key, out N node, T1 arg1, T2 arg2, bool isPool = true)
			where N : class, INode
			where B : class, IBranch<K>
		{
			if (this.AddBranch<B>().TryAddNode(key, out node, isPool))
			{
				node.SetActive(true);//激活节点
				node.TrySendRule(IAwakeRule<T1, T2>.Default, arg1, arg2);
				node.OnTreeAddSelf();
			}
			return node;
		}

		public virtual N TreeAddNode<B, K, N, T1, T2, T3>(K key, out N node, T1 arg1, T2 arg2, T3 arg3, bool isPool = true)
			where N : class, INode
			where B : class, IBranch<K>
		{
			if (this.AddBranch<B>().TryAddNode(key, out node, isPool))
			{
				node.SetActive(true);//激活节点
				node.TrySendRule(IAwakeRule<T1, T2, T3>.Default, arg1, arg2, arg3);
				node.OnTreeAddSelf();
			}
			return node;
		}

		public virtual N TreeAddNode<B, K, N, T1, T2, T3, T4>(K key, out N node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, bool isPool = true)
			where N : class, INode
			where B : class, IBranch<K>
		{
			if (this.AddBranch<B>().TryAddNode(key, out node, isPool))
			{
				node.SetActive(true);//激活节点
				node.TrySendRule(IAwakeRule<T1, T2, T3, T4>.Default, arg1, arg2, arg3, arg4);
				node.OnTreeAddSelf();
			}
			return node;
		}

		public virtual N TreeAddNode<B, K, N, T1, T2, T3, T4, T5>(K key, out N node, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, bool isPool = true)
			where N : class, INode
			where B : class, IBranch<K>
		{
			if (this.AddBranch<B>().TryAddNode(key, out node, isPool))
			{
				node.SetActive(true);//激活节点
				node.TrySendRule(IAwakeRule<T1, T2, T3, T4, T5>.Default, arg1, arg2, arg3, arg4, arg5);
				node.OnTreeAddSelf();
			}
			return node;
		}

		public virtual void OnTreeAddSelf()
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

		public void OnBeforeDispose()
		{
			this.Core.BeforeRemoveRuleGroup?.Send(this);
		}

		public virtual void OnDispose()//未完
		{
			this.RemoveInParentBranch();//从父节点分支移除
			this.SendAllReferencedNodeRemove();//_判断移除引用关系 X
			this.SetActive(false);//激活变更
			this.Core.DisableRuleGroup?.Send(this); //禁用事件通知 X
			if (this is INodeListener nodeListener && this is not ICoreNode)
			{
				//检测移除静态监听
				this.Core.ReferencedPoolManager.RemoveStaticListener(nodeListener);
				//检测移除动态监听
				this.Core.ReferencedPoolManager.RemoveDynamicListener(nodeListener);
			}
			this.Core.RemoveRuleGroup?.Send(this);//移除事件通知
			if (this is not ICoreNode)//广播给全部监听器通知 X
			{
				this.GetListenerActuator<IListenerRemoveRule>()?.Send((INode)this);
			}
			this.Core.ReferencedPoolManager.Remove(this);//引用池移除 ?
			this.DisposeDomain(); //清除域节点
			this.Parent = null;//清除父节点
			Core?.Recycle(this);//回收到池
		}

		#endregion

		#region 嫁接

		public virtual bool TreeGraftNode<B, K, N>(K key, N node)
			where N : class, INode
			where B : class, IBranch<K>
		{
			if (!this.AddBranch<B>().TryGraftNode(key, node)) return false;
			node.RefreshActive();
			node.TraversalLevel(current => current.OnTreeGraftSelf());
			return true;
		}

		public virtual void OnTreeGraftSelf()//id相同数据同步？
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
			this.SendRule(DefaultType<IGraftRule>.Default);
		}

		#endregion

		#region 裁剪

		public virtual void TreeCutSelf()
		{
			if (this.IsRecycle) return; //是否已经回收
			this.TraversalPostorder(current => current.OnTreeCutSelf());
			this.RemoveInParentBranch();//从父节点分支移除
		}
		public virtual void OnTreeCutSelf()
		{
			this.SendAllReferencedNodeRemove();//_判断移除引用关系 X
			if (this is INodeListener nodeListener && this is not ICoreNode)
			{
				//检测移除静态监听
				this.Core.ReferencedPoolManager.RemoveStaticListener(nodeListener);
				//检测移除动态监听
				this.Core.ReferencedPoolManager.RemoveDynamicListener(nodeListener);
			}
			this.SendRule(DefaultType<ICutRule>.Default);
			if (this is not ICoreNode)//广播给全部监听器通知 X
			{
				this.GetListenerActuator<IListenerRemoveRule>()?.Send((INode)this);
			}
			this.Core.ReferencedPoolManager.Remove(this);//引用池移除 ?
		}

		#endregion

		#endregion

	}
}
