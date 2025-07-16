/****************************************

* 作者：闪电黑客
* 日期：2025/7/10 11:42

* 描述：INode接口代理实现

* 代理实现INode接口方法，为了方便生成Node部分类代码

*/

namespace WorldTree
{



	/// <summary>
	/// INode接口代理实现
	/// </summary>
	public static partial class INodeProxyRule
	{
		/// <summary>
		/// 设置激活
		/// </summary>
		public static void SetActive(INode self, bool value)
		{
			if (self.ActiveToggle != value)
			{
				self.ActiveToggle = value;
				self.RefreshActive();
			}
		}

		/// <summary>
		/// 刷新当前节点激活状态：层序遍历设置子节点
		/// </summary>
		public static void RefreshActive(INode self)
		{
			//如果状态相同，不需要刷新
			if (self.IsActive == ((self.Parent == null) ? self.ActiveToggle : self.Parent.IsActive && self.ActiveToggle)) return;

			//层序遍历设置子节点
			using (self.Core.PoolGetUnit(out UnitQueue<INode> queue))
			{
				queue.Enqueue(self);
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


		/// <summary>
		/// 字符串化当前节点
		/// </summary>
		public static string ToString(INode self)
		{
			return self.GetType().ToString();
		}

		#region 节点处理

		#region 创建

		/// <summary>
		/// 创建时：Node的Id获取和法则支持
		/// </summary>
		public static void OnCreate(INode self)
		{
			self.InstanceId = self.Core.IdManager.GetId();
			self.Id = self.InstanceId;
			self.Core.RuleManager?.SupportNodeRule(self.Type);
		}

		#endregion

		#region 添加

		/// <summary>
		/// 尝试将自身添加到树结构（代理方法）
		/// </summary>
		public static bool TryAddSelfToTree<B, K>(this INode self, K key, INode parent)
			where B : class, IBranch<K>
		{
			if (NodeBranchHelper.AddBranch<B>(parent).TryAddNode(key, self))
			{
				self.BranchType = self.Core.TypeToCode<B>();
				self.Parent = parent;
				self.Core = parent.Core;
				self.World = parent.World;
				self.SetActive(true);//激活节点
				AddNodeView(self);
				return true;
			}
			return false;
		}

		/// <summary>
		/// 添加时
		/// </summary>
		/// <summary>
		/// 节点加入树结构时的处理（代理方法）
		/// </summary>
		public static void OnAddSelfToTree(this INode self)
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
			if (self.IsActive != self.activeEventMark)//激活变更
			{
				if (self.IsActive)
				{
					self.Core.EnableRuleGroup?.Send(self);//激活事件通知
				}
				else
				{
					self.Core.DisableRuleGroup?.Send(self); //禁用事件通知
				}
			}
			self.Core.AddRuleGroup?.Send(self);//节点添加事件通知
		}

		#endregion

		#region 移除

		/// <summary>
		/// 释放所有分支的所有节点（代理方法）
		/// </summary>
		public static void RemoveAllNode(INode self)
		{
			if (self.BranchDict == null) return;
			using (self.Core.PoolGetUnit(out UnitStack<IBranch> branchs))
			{
				foreach (var item in self.BranchDict) branchs.Push(item.Value);
				while (branchs.Count != 0) self.RemoveAllNode(branchs.Pop().Type);
			}

			//假如在分支移除过程中，节点又添加了新的分支。那么就是错误的，新增分支将无法回收。
			if (self.BranchDict.Count != 0)
			{
				foreach (var item in self.BranchDict)
				{
					self.Log($"移除分支出错，意外的新分支，节点：{self} 分支:{item.GetType()}");
				}
			}
		}


		/// <summary>
		/// 释放指定分支类型的所有节点（代理方法）
		/// </summary>
		public static void RemoveAllNode(INode self, long branchType)
		{
			if (NodeBranchHelper.TryGetBranch(self, branchType, out IBranch branch))
			{
				if (branch.Count != 0)
				{
					// 迭代器无法一边迭代一边删除，这里用栈存储需要删除的节点
					using (self.Core.PoolGetUnit(out UnitStack<INode> nodes))
					{
						foreach (var item in branch) nodes.Push(item);
						while (nodes.Count != 0) nodes.Pop().Dispose();
					}

					// 如果在节点移除过程中又添加了新节点，将无法回收
					if (branch.Count != 0)
					{
						foreach (var item in branch)
						{
							self.LogError($"移除节点出错，意外的新节点，父级:{self.GetType()} 分支: {branch.GetType()} 节点:{item.GetType()}:{item.Id}");
						}
					}
				}
			}
		}

		/// <summary>
		/// 释放节点
		/// </summary>
		public static void Dispose(INode self)
		{
			//是否已经释放
			if (self.IsDisposed) return;

			//节点释放前序遍历处理,节点释放后续遍历处理
			NodeBranchTraversalHelper.TraversalPrePostOrder(self, current => current.OnBeforeDispose(), current => current.OnDispose());
		}

		/// <summary>
		/// 释放前的处理（代理方法）
		/// </summary>
		public static void OnBeforeDispose(INode self) => self.Core.BeforeRemoveRuleGroup?.Send(self);


		/// <summary>
		/// 节点释放时的处理（代理方法）
		/// </summary>
		public static void OnDispose(INode self)
		{
			self.ViewBuilder?.Core.WorldContext.Post(self.ViewBuilderDispose);
			NodeBranchHelper.RemoveNode(self); // 从父节点分支移除
			self.SetActive(false); // 激活变更
			self.Core.DisableRuleGroup?.Send(self); // 禁用事件通知
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

			//self.DisposeDomain(); //清除域节点
			self.Parent = null; // 清除父节点
			self.Core.PoolRecycle(self); // 回收到池
		}

		#endregion

		#region 嫁接

		/// <summary>
		/// 节点嫁接到树结构（代理方法）
		/// </summary>
		public static bool TryGraftSelfToTree<B, K>(INode self, K key, INode parent)
			where B : class, IBranch<K>
			=> self.TryGraftSelfToTree(self.TypeToCode<B>(), key, parent);


		/// <summary>
		/// 节点嫁接到树结构，无约束条件（代理方法）
		/// </summary>
		public static bool TryGraftSelfToTree<K>(this INode self, long branchType, K key, INode parent)
		{
			if (NodeBranchHelper.AddBranch(parent, branchType) is not IBranch<K> branch) return false;
			if (!branch.TryAddNode(key, self)) return false;

			self.BranchType = branch.Type;
			self.Parent = parent;
			self.Core = parent.Core;
			self.World = parent.World;

			self.RefreshActive();
			NodeBranchTraversalHelper.TraversalPrePostOrder(self, current => current.OnBeforeGraftSelfToTree(), current => current.OnGraftSelfToTree());
			return true;
		}

		/// <summary>
		/// 节点嫁接前的处理（代理方法）
		/// </summary>
		public static void OnBeforeGraftSelfToTree(this INode self)
		{
			self.Core = self.Parent.Core;
			self.World = self.Parent.World;
			// 序列化时，需要重新设置所有节点的父节点
			if (self.IsSerialize)
			{
				if (self.BranchDict != null)
				{
					foreach (var brancItem in self.BranchDict)
					{
						if (brancItem.Value == null) continue;
						foreach (var nodeItem in brancItem.Value)
						{
							nodeItem.Parent = self;
							nodeItem.BranchType = brancItem.Value.Type;
						}
					}
				}
			}
			AddNodeView(self);
		}

		/// <summary>
		/// 节点嫁接的处理（代理方法）
		/// </summary>
		public static void OnGraftSelfToTree(this INode self)
		{
			self.Core.ReferencedPoolManager.TryAdd(self);//添加到引用池
			if (self is not IListenerIgnorer)//广播给全部监听器
			{
				IRuleExecutor<ListenerAdd> ruleActuator = NodeListenerExecutorHelper.GetListenerExecutor<ListenerAdd>(self);
				ruleActuator?.Send(self);
			}
			if (self is INodeListener nodeListener && self is not IListenerIgnorer)//检测添加静态监听
			{
				self.Core.ReferencedPoolManager.TryAddListener(nodeListener);
			}

			if (self.IsSerialize)
			{
				self.Core.DeserializeRuleGroup?.Send(self);//反序列化事件通知
				self.IsSerialize = false;
			}

			if (self.IsActive != self.activeEventMark)//激活变更
			{
				if (self.IsActive)
				{
					self.Core.EnableRuleGroup?.Send(self);//激活事件通知
				}
				else
				{
					self.Core.DisableRuleGroup?.Send(self); //禁用事件通知
				}
			}
			if (!self.IsSerialize) self.Core.GraftRuleGroup?.Send(self);//嫁接事件通知
		}

		#endregion

		#region 裁剪

		/// <summary>
		/// 从树上将自己裁剪下来（代理方法）
		/// </summary>
		public static INode CutSelf(this INode self)
		{
			if (self.IsDisposed) return null; // 是否已经回收
			if (self.Parent == null) return self;
			NodeBranchTraversalHelper.TraversalPostorder(self, current => current.OnCutSelf());
			NodeBranchHelper.RemoveNode(self); // 从父节点分支移除
			return self;
		}

		/// <summary>
		/// 从树上将自己裁剪下来时的处理（代理方法）
		/// </summary>
		public static void OnCutSelf(this INode self)
		{
			self.ViewBuilder?.Dispose();
			self.ViewBuilder = null;
			if (self is INodeListener nodeListener && self is not IListenerIgnorer)
			{
				// 检测移除静态监听
				self.Core.ReferencedPoolManager.RemoveListener(nodeListener);
			}
			self.Core.CutRuleGroup?.Send(self); // 裁剪事件通知
			if (self is not IListenerIgnorer) // 广播给全部监听器通知
			{
				NodeListenerExecutorHelper.GetListenerExecutor<ListenerRemove>(self)?.Send(self);
			}
			self.Core.ReferencedPoolManager.Remove(self); // 引用池移除
			self.Parent = null; // 清除父节点
		}

		#endregion

		/// <summary>
		/// 添加节点可视化
		/// </summary>
		public static void AddNodeView(INode self)
		{
			self.ViewBuilder?.Core.WorldContext.Post(self.ViewBuilderDispose);
			self.Core.ViewBuilder?.Core.WorldContext.Post(self.ViewBuilderCreate);
		}

		/// <summary>
		/// 创建节点可视化
		/// </summary>
		private static void ViewBuilderCreate(this INode self)
		{
			if (self.Parent?.ViewBuilder == null)
			{
				self.ViewBuilder = null;
				return;
			}
			// 拿到父节点的可视化生成器的父级节点
			INode viewParent = self.Parent.ViewBuilder.Parent;

			// 生成自身的可视化生成器
			INode nodeView = viewParent.Core.PoolGetNode(self.Parent.ViewBuilder.Type);

			// 将自身添加到父节点的可视化生成器中，而可视化则挂到可视化父级节点上
			self.ViewBuilder = NodeBranchHelper.AddNodeToTree(viewParent, default(ChildBranch), nodeView.Id, nodeView, (INode)self, (INode)self.Parent) as IWorldTreeNodeViewBuilder;
		}

		/// <summary>
		/// 释放节点可视化
		/// </summary>
		private static void ViewBuilderDispose(this INode self)
		{
			self.ViewBuilder.Dispose();
			self.ViewBuilder = null;
		}

		#endregion
	}
}