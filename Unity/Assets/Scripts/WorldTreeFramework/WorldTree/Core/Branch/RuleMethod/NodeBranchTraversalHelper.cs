using System;

namespace WorldTree
{
	/// <summary>
	/// 节点分支遍历帮助类
	/// </summary>
	public static class NodeBranchTraversalHelper
	{
		/// <summary>
		/// 前序遍历
		/// </summary>
		public static INode TraversalPreorder(INode self, Action<INode> action)
		{
			//自己没有分支
			if (self.BranchDict == null || self.BranchDict.Count == 0)
			{
				action(self);
				return self;
			}

			// 当前节点
			INode current;
			// 从对象池拿栈
			UnitStack<INode> stack = self.Core.PoolGetUnit<UnitStack<INode>>();
			stack.Push(self);
			while (stack.Count != 0)
			{
				current = stack.Pop();
				action(current);
				if (current.BranchDict != null)
				{
					foreach (var branchs in current.BranchDict)
					{
						foreach (INode node in branchs.Value)
						{
							stack.Push(node);
						}
					}
				}
			}
			stack.Dispose();
			return self;
		}

		/// <summary>
		/// 层序遍历
		/// </summary>
		public static INode TraversalLevel(INode self, Action<INode> action)
		{
			//自己没有分支
			if (self.BranchDict == null || self.BranchDict.Count == 0)
			{
				action(self);
				return self;
			}

			// 从对现场拿队列
			UnitQueue<INode> queue = self.Core.PoolGetUnit<UnitQueue<INode>>();
			queue.Enqueue(self);

			while (queue.Count != 0)
			{
				// 当前节点
				var current = queue.Dequeue();

				action(current);

				if (current.BranchDict != null)
				{
					foreach (var branchs in current.BranchDict)
					{
						foreach (INode node in branchs.Value)
						{
							queue.Enqueue(node);
						}
					}
				}
			}
			queue.Dispose();
			return self;
		}

		/// <summary>
		/// 后序遍历
		/// </summary>
		public static INode TraversalPostorder(INode self, Action<INode> action)
		{
			//自己没有分支
			if (self.BranchDict == null || self.BranchDict.Count == 0)
			{
				action(self);
				return self;
			}
			// 当前节点
			INode current;
			// 从对象池拿栈，用于存放一个分支的节点
			UnitStack<INode> stack = self.Core.PoolGetUnit<UnitStack<INode>>();
			// 从对象池拿栈，用于存放所有节点
			UnitStack<INode> allStack = self.Core.PoolGetUnit<UnitStack<INode>>();
			stack.Push(self);
			while (stack.Count != 0)
			{
				current = stack.Pop();
				allStack.Push(current);
				if (current.BranchDict != null)
				{
					foreach (var branchs in current.BranchDict)
					{
						foreach (INode node in branchs.Value)
						{
							stack.Push(node);
						}
					}
				}
			}
			stack.Dispose();
			while (allStack.Count != 0)
			{
				action(allStack.Pop());
			}
			allStack.Dispose();
			return self;
		}

		/// <summary>
		/// 前后双序遍历
		/// </summary>
		public static INode TraversalPrePostOrder(INode self, Action<INode> preAction, Action<INode> postAction)
		{
			//自己没有分支
			if (self.BranchDict == null || self.BranchDict.Count == 0)
			{
				preAction(self);
				postAction(self);
				return self;
			}
			// 当前节点
			INode current;
			// 从对象池拿栈，用于存放一个分支的节点
			UnitStack<INode> stack = self.Core.PoolGetUnit<UnitStack<INode>>();
			// 从对象池拿栈，用于存放所有节点
			UnitStack<INode> allStack = self.Core.PoolGetUnit<UnitStack<INode>>();
			stack.Push(self);
			while (stack.Count != 0)
			{
				current = stack.Pop();
				preAction(current);
				allStack.Push(current);
				if (current.BranchDict != null)
				{
					foreach (var branchs in current.BranchDict)
					{
						foreach (INode node in branchs.Value)
						{
							stack.Push(node);
						}
					}
				}
			}
			stack.Dispose();
			while (allStack.Count != 0)
			{
				postAction(allStack.Pop());
			}
			allStack.Dispose();
			return self;
		}
	}
}
