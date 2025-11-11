using System;
using System.Collections.Generic;

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
		public static INode TraversalPreorder(INode self, Action<INode> action, bool isSelf = true)
		{
			//自己没有分支
			if (self.BranchDict == null || self.BranchDict.BranchCount == 0)
			{
				if (isSelf) action(self);
				return self;
			}

			// 当前节点
			INode current;
			// 从对象池拿栈
			UnitStack<INode> nodeStack = self.Core.PoolGetUnit<UnitStack<INode>>();
			if (isSelf) nodeStack.Push(self);
			while (nodeStack.Count != 0)
			{
				current = nodeStack.Pop();
				action(current);
				if (current.BranchDict != null)
				{
					foreach (var branch in current.BranchDict)
					{
						foreach (INode node in (IEnumerable<INode>)branch)
						{
							nodeStack.Push(node);
						}
					}
				}
			}
			nodeStack.Dispose();
			return self;
		}

		/// <summary>
		/// 层序遍历
		/// </summary>
		public static INode TraversalLevel(INode self, Action<INode> action, bool isSelf = true)
		{
			//自己没有分支
			if (self.BranchDict == null || self.BranchDict.BranchCount == 0)
			{
				if (isSelf) action(self);
				return self;
			}

			// 从对现场拿队列
			UnitQueue<INode> nodeQueue = self.Core.PoolGetUnit<UnitQueue<INode>>();
			if (isSelf) nodeQueue.Enqueue(self);

			while (nodeQueue.Count != 0)
			{
				// 当前节点
				var current = nodeQueue.Dequeue();

				action(current);

				if (current.BranchDict != null)
				{
					foreach (var branch in current.BranchDict)
					{
						foreach (INode node in (IEnumerable<INode>)branch)
						{
							nodeQueue.Enqueue(node);
						}
					}
				}
			}
			nodeQueue.Dispose();
			return self;
		}

		/// <summary>
		/// 后序遍历
		/// </summary>
		public static INode TraversalPostorder(INode self, Action<INode> action, bool isSelf = true)
		{
			//自己没有分支
			if (self.BranchDict == null || self.BranchDict.BranchCount == 0)
			{
				if (isSelf) action(self);
				return self;
			}
			// 当前节点
			INode current;
			// 从对象池拿栈，用于存放一个分支的节点
			UnitStack<INode> nodeStack = self.Core.PoolGetUnit<UnitStack<INode>>();
			// 从对象池拿栈，用于存放所有节点
			UnitStack<INode> allStack = self.Core.PoolGetUnit<UnitStack<INode>>();
			if (isSelf) nodeStack.Push(self);
			while (nodeStack.Count != 0)
			{
				current = nodeStack.Pop();
				allStack.Push(current);
				if (current.BranchDict != null)
				{
					foreach (var branch in current.BranchDict)
					{
						foreach (INode node in (IEnumerable<INode>)branch)
						{
							nodeStack.Push(node);
						}
					}
				}
			}
			nodeStack.Dispose();
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
		public static INode TraversalPrePostOrder(INode self, Action<INode> preAction, Action<INode> postAction, bool isSelf = true)
		{
			//自己没有分支
			if (self.BranchDict == null || self.BranchDict.BranchCount == 0)
			{
				if (isSelf)
				{
					preAction(self);
					postAction(self);
				}
				return self;
			}
			// 当前节点
			INode current;
			// 从对象池拿栈，用于存放一个分支的节点
			UnitStack<INode> nodeStack = self.Core.PoolGetUnit<UnitStack<INode>>();
			// 从对象池拿栈，用于存放所有节点
			UnitStack<INode> allStack = self.Core.PoolGetUnit<UnitStack<INode>>();
			if (isSelf) nodeStack.Push(self);
			while (nodeStack.Count != 0)
			{
				current = nodeStack.Pop();
				preAction(current);
				allStack.Push(current);
				if (current.BranchDict != null)
				{
					foreach (var branch in current.BranchDict)
					{
						foreach (INode node in (IEnumerable<INode>)branch)
						{
							nodeStack.Push(node);
						}
					}
				}
			}
			nodeStack.Dispose();
			while (allStack.Count != 0)
			{
				postAction(allStack.Pop());
			}
			allStack.Dispose();
			return self;
		}
	}
}
