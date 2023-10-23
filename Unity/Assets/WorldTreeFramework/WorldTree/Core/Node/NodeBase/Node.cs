/****************************************

* 创 建 者：  闪电黑客
* 创建时间：  2022/5/6 21:27
* 描    述:   世界树节点基类

****************************************/

using Codice.Client.Common.TreeGrouper;
using System;
using static Codice.CM.Common.CmCallContext;

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


		#endregion

		public override string ToString()
		{
			return GetType().ToString();
		}

		#region 嫁接

		public virtual void TreeGraftSelf()
		{
			this.RefreshActive();
			this.TraversalLevel(current => current.OnTreeGraftSelf());
		}

		public virtual void OnTreeGraftSelf()
		{
			this.Core = this.Parent.Core;
			this.Root = this.Parent.Root;
			if (this.Domain != this) this.Domain = this.Parent.Domain;
			this.Core.ReferencedPoolManager.TryAdd(this);//添加到引用池
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
			if (this is not ICoreNode)//广播给全部监听器
			{
				this.GetListenerActuator<IListenerAddRule>()?.Send((INode)this);
			}
			if (this is INodeListener nodeListener && this is not ICoreNode)//检测添加静态监听
			{
				this.Core.ReferencedPoolManager.TryAddStaticListener(nodeListener);
			}
			this.SendRule(DefaultType<IGraftRule>.Default);
		}

		#endregion

		#region 裁剪

		public virtual void TreeCutSelf()//为什么是写给Node而不是Branch
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


		#region 添加

		public virtual void TreeAddSelf()
		{
			this.Core = this.Parent.Core;
			this.Root = this.Parent.Root;
			if (this.Domain != this) this.Domain = this.Parent.Domain;
			this.SetActive(true);//激活变更
			this.Core.EnableRuleGroup?.Send(this);//激活事件通知
			this.OnTreeAddSelf();
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

		public virtual void OnDispose()
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
	}

}
