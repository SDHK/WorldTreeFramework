/****************************************

* 作者： 闪电黑客
* 日期： 2025/10/16 19:49

* 描述： 视图适配绑定基类
* 
* 作为具体UI组件的桥接，负责将具体UI组件与视图数据进行绑定
* 本身不包含具体UI逻辑，所有UI逻辑应在具体的View中实现

*/
namespace WorldTree
{
	/// <summary>
	/// 视图适配绑定基类, 桥接具体的UI组件
	/// </summary>
	public class ViewBind : Node
		, ComponentOf<ViewManager>
		, ComponentOf<View>
		, AsComponentBranch
		, AsChildBranch
		, AsRule<Awake>
		, AsRule<ViewRegister>
		, AsRule<ViewUnRegister>
	{
		/// <summary>
		/// UI实例
		/// </summary>
		public IViewElement ui;

		public override void OnAddSelfToTree() => ViewBindProxyRule.OnAddSelfToTree(this);

		public override void Dispose() => ViewBindProxyRule.Dispose(this);

		public override void OnBeforeDispose() => ViewBindProxyRule.OnBeforeDispose(this);

		public override void OnDispose() => ViewBindProxyRule.OnDispose(this);
	}

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
			self.World.Line.ReferencedPoolManager.TryAdd(self);//添加到引用池
			if (self is not IListenerIgnorer)//广播给全部监听器
			{
				IRuleExecutor<ListenerAdd> ruleActuator = NodeListenerExecutorHelper.GetListenerExecutor<ListenerAdd>(self);
				ruleActuator?.Send(self);
			}
			if (self is INodeListener nodeListener && self is not IListenerIgnorer)//检测自身是否为监听器
			{
				self.World.Line.ReferencedPoolManager.TryAddListener(nodeListener);
			}

			// View 的 Bind 字段在任何事件发出前就位
			if (self.Parent is View view) view.Bind = (ViewBind)self;

			NodeRuleHelper.TrySendRule(self, default(ViewRegister));

			NodeRuleHelper.TrySendRule(self.Parent, default(Open));
			NodeRuleHelper.TrySendRule(self.Parent, default(LayerChange));

			if (self.IsActive != self.activeEventMark)//激活变更
			{
				if (self.IsActive)
				{
					self.World.Line.Core.EnableRuleGroup?.Send(self); //激活事件通知
					NodeRuleHelper.TrySendRule(self.Parent, default(Show));
				}
				else
				{
					self.World.Line.Core.DisableRuleGroup?.Send(self); //禁用事件通知
					NodeRuleHelper.TrySendRule(self.Parent, default(Hide));
				}
			}
			self.World.Line.Core.AddRuleGroup?.Send(self);//节点添加事件通知
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
					self.World.Line.Core.EnableRuleGroup?.Send(self);//激活事件通知
					NodeRuleHelper.TrySendRule(self.Parent, default(Show));
				}
				else
				{
					self.World.Line.Core.DisableRuleGroup?.Send(self); //禁用事件通知
					NodeRuleHelper.TrySendRule(self.Parent, default(Hide));
				}
			}
		}
		/// <summary>
		/// 销毁前调用
		/// </summary>
		public static void OnBeforeDispose(INode self) => self.World.Line.Core.BeforeRemoveRuleGroup?.Send(self);
		/// <summary>
		/// 销毁时调用 
		/// </summary>
		[INodeThis]
		public static void OnDispose(INode self)
		{
			if (self.ViewBuilder != null)
			{
				var builder = self.ViewBuilder;
				self.ViewBuilder.World.Line.WorldContext.Post(builder.Dispose);
				self.ViewBuilder = null;
			}

			NodeRuleHelper.TrySendRule(self, default(ViewUnRegister));
			NodeRuleHelper.TrySendRule(self.Parent, default(Close));

			NodeBranchHelper.RemoveNode(self); // 从父节点分支移除

			if (self is INodeListener nodeListener && self is not IListenerIgnorer) // 检测自身为监听器
			{
				self.World.Line.ReferencedPoolManager.RemoveListener(nodeListener);
			}
			self.World.Line.Core.RemoveRuleGroup?.Send(self); // 移除事件通知

			if (self is not IListenerIgnorer) // 广播给全部监听器通知
			{
				NodeListenerExecutorHelper.GetListenerExecutor<ListenerRemove>(self)?.Send(self);
			}
			self.World.Line.ReferencedPoolManager.Remove(self); // 引用池移除

			self.Parent = null; // 清除父节点
			self.World.PoolRecycle(self); // 回收到池
		}
	}
}
