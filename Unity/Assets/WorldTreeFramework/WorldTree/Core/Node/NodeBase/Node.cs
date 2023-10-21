/****************************************

* 创 建 者：  闪电黑客
* 创建时间：  2022/5/6 21:27
* 描    述:   世界树节点基类

****************************************/

using System;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
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
	public abstract partial class Node : INode
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


		#region 添加

		public virtual void TreeAddSelf()
		{
			this.RefreshActive();
			this.TraversalLevel
			(
				(INode current) =>
				{
					current.Core = current.Parent.Core;
					current.Root = current.Parent.Root;
					if (current.Domain != current) current.Domain = current.Parent.Domain;
					current.Core.ReferencedPoolManager.TryAdd(current);//添加到引用池
					if (current.IsActive != current.m_ActiveEventMark)//激活变更
					{
						if (current.IsActive)
						{
							current.Core.EnableRuleGroup?.Send(current);//激活事件通知
						}
						else
						{
							current.Core.DisableRuleGroup?.Send(current); //禁用事件通知
						}
					}
					if (current is not ICoreNode)//广播给全部监听器
					{
						current.GetListenerActuator<IListenerAddRule>()?.Send(current);
					}
					if (current is INodeListener nodeListener && current is not ICoreNode)//检测添加静态监听
					{
						current.Core.ReferencedPoolManager.TryAddStaticListener(nodeListener);
					}
					current.Core.AddRuleGroup?.Send(current);//节点添加事件通知
				}
			);
		}

		#endregion

		#region 移除

		public virtual void TreeRemoveSelf()
		{
			if (this.IsRecycle) return; //是否已经回收

			this.TraversalPostorder
			(
				(INode current) =>
				{
					current.SendAllReferencedNodeRemove();//_判断移除引用关系 X
					if (current is INodeListener nodeListener && current is not ICoreNode)
					{
						//检测移除静态监听
						current.Core.ReferencedPoolManager.RemoveStaticListener(nodeListener);
						//检测移除动态监听
						current.Core.ReferencedPoolManager.RemoveDynamicListener(nodeListener);
					}
					if (current is not ICoreNode)//广播给全部监听器通知 X
					{
						current.GetListenerActuator<IListenerRemoveRule>()?.Send(current);
					}
					current.Core.ReferencedPoolManager.Remove(current);//引用池移除 ?
				}
			);
			this.RemoveInParentBranch();//从父节点分支移除
		}
		#endregion


		#region 释放


		/// <summary>
		/// 回收节点的处理
		/// </summary>
		public virtual void Dispose()
		{
			if (this.IsRecycle || this.IsDisposed) return; //是否已经回收
			this.TraversalPrePostOrder
			(
				(INode current) =>//节点回收前序遍历处理
				{
					current.Core.BeforeRemoveRuleGroup?.Send(current);
				}
			,
				(INode current) =>//节点回收后续遍历处理
				{
					current.RemoveInParentBranch();//从父节点分支移除
					current.SendAllReferencedNodeRemove();//_判断移除引用关系 X
					current.SetActive(false);//激活变更
					current.Core.DisableRuleGroup?.Send(current); //禁用事件通知 X
					if (current is INodeListener nodeListener && current is not ICoreNode)
					{
						//检测移除静态监听
						current.Core.ReferencedPoolManager.RemoveStaticListener(nodeListener);
						//检测移除动态监听
						current.Core.ReferencedPoolManager.RemoveDynamicListener(nodeListener);
					}
					current.Core.RemoveRuleGroup?.Send(current);//移除事件通知
					if (current is not ICoreNode)//广播给全部监听器通知 X
					{
						current.GetListenerActuator<IListenerRemoveRule>()?.Send(current);
					}
					current.Core.ReferencedPoolManager.Remove(current);//引用池移除 ?
					current.DisposeDomain(); //清除域节点
					current.Parent = null;//清除父节点
					current.OnDispose();//释放或回收到池
				}
			);
		}

		/// <summary>
		/// 释放后：回收到对象池
		/// </summary>
		public virtual void OnDispose()
		{
			Core?.Recycle(this);
		}
		#endregion
	}

}
