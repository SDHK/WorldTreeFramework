namespace WorldTree
{

	/// <summary>
	/// 视图接口代理 
	/// </summary>
	public static class ViewBindProxyRule
	{
		/// <summary>
		/// 添加时
		/// </summary>
		[INodeThis]
		public static void OnAddSelfToTree(INode self)
		{
			self.Core.ReferencedPoolManager.TryAdd(self);//添加到引用池
			if (self is not IListenerIgnorer)//广播给全部监听器
			{
				IRuleExecutor<ListenerAdd> ruleActuator = NodeListenerExecutorHelper.GetListenerExecutor<ListenerAdd>(self);
				ruleActuator?.Send(self);
			}
			if (self is INodeListener nodeListener && self is not IListenerIgnorer)//检测自身是否为监听器
			{
				self.Core.ReferencedPoolManager.TryAddListener(nodeListener);
			}

			NodeRuleHelper.TrySendRule(self, default(ViewElementLoad));
			NodeRuleHelper.TrySendRule(self, default(ViewRegister));

			NodeRuleHelper.TrySendRule(self.Parent, default(Open));
			NodeRuleHelper.TrySendRule(self.Parent, default(LayerChange));


			if (self.IsActive != self.activeEventMark)//激活变更
			{
				if (self.IsActive)
				{
					self.Core.EnableRuleGroup?.Send(self); //激活事件通知
					NodeRuleHelper.TrySendRule(self.Parent, default(Show));
				}
				else
				{
					self.Core.DisableRuleGroup?.Send(self); //禁用事件通知
					NodeRuleHelper.TrySendRule(self.Parent, default(Hide));
				}
			}
			self.Core.AddRuleGroup?.Send(self);//节点添加事件通知
		}

		/// <summary>
		/// 释放节点
		/// </summary>
		public static void Dispose(INode self)
		{
			//是否已经释放
			if (self.IsDisposed) return;
			//后序遍历处理节点激活
			NodeBranchTraversalHelper.TraversalPostorder(self, node => node.OnBeforeDisposeActive());
			//节点释放前序遍历处理,节点释放后续遍历处理
			NodeBranchTraversalHelper.TraversalPrePostOrder(self, node => node.OnBeforeDispose(), node => node.OnDispose());
		}

		/// <summary>
		/// 销毁前调用激活设置 
		/// </summary>
		private static void OnBeforeDisposeActive(this INode self)
		{
			self.SetActive(false); // 激活变更

			if (self.IsActive != self.activeEventMark)//激活变更
			{
				if (self.IsActive)
				{
					self.Core.EnableRuleGroup?.Send(self);//激活事件通知
					NodeRuleHelper.TrySendRule(self.Parent, default(Show));
				}
				else
				{
					self.Core.DisableRuleGroup?.Send(self); //禁用事件通知
					NodeRuleHelper.TrySendRule(self.Parent, default(Hide));
				}
			}
		}
		/// <summary>
		/// 销毁前调用 
		/// </summary>
		public static void OnBeforeDispose(INode self)
		{
			NodeRuleHelper.TrySendRule(self.Parent, default(SubViewClose), self as View);
			self.OnBeforeDispose();
			NodeRuleHelper.TrySendRule(self.Parent, default(Close));
		}
		/// <summary>
		/// 销毁时调用 
		/// </summary>
		[INodeThis]
		public static void OnDispose(INode self)
		{
			NodeRuleHelper.TrySendRule(self, default(ViewUnRegister));
			NodeRuleHelper.TrySendRule(self, default(ViewElementUnLoad));
			self.ViewBuilder?.Core.WorldContext.Post(self.ViewBuilderDispose);
			NodeBranchHelper.RemoveNode(self); // 从父节点分支移除

			if (self is INodeListener nodeListener && self is not IListenerIgnorer) // 检测自身为监听器
			{
				self.Core.ReferencedPoolManager.RemoveListener(nodeListener);
			}
			self.Core.RemoveRuleGroup?.Send(self); // 移除事件通知

			if (self is not IListenerIgnorer) // 广播给全部监听器通知
			{
				NodeListenerExecutorHelper.GetListenerExecutor<ListenerRemove>(self)?.Send(self);
			}
			self.Core.ReferencedPoolManager.Remove(self); // 引用池移除

			self.Parent = null; // 清除父节点
			self.Core.PoolRecycle(self); // 回收到池
		}

		/// <summary>
		/// 释放节点可视化
		/// </summary>
		private static void ViewBuilderDispose(this INode self)
		{
			self.ViewBuilder.Dispose();
			self.ViewBuilder = null;
		}

	}
}
