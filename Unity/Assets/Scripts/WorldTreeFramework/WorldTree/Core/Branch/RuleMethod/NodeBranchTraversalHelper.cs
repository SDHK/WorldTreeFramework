using System;

namespace WorldTree
{
	public static class NodeBranchTraversalHelper
	{
		/// <summary>
		/// 前序遍历
		/// </summary>
		public static INode TraversalPreorder(INode self, Action<INode> action)
		{
			//自己没有分支
			if (self.m_Branchs == null || self.m_Branchs.Count == 0)
			{
				action(self);
				return self;
			}

			INode current;
			UnitStack<INode> stack = self.Core.PoolGetUnit<UnitStack<INode>>();
			stack.Push(self);
			while (stack.Count != 0)
			{
				current = stack.Pop();
				action(current);
				if (current.m_Branchs != null)
				{
					foreach (var branchs in current.m_Branchs)
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
			if (self.m_Branchs == null || self.m_Branchs.Count == 0)
			{
				action(self);
				return self;
			}

			UnitQueue<INode> queue = self.Core.PoolGetUnit<UnitQueue<INode>>();
			queue.Enqueue(self);

			while (queue.Count != 0)
			{
				var current = queue.Dequeue();

				action(current);

				if (current.m_Branchs != null)
				{
					foreach (var branchs in current.m_Branchs)
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
			if (self.m_Branchs == null || self.m_Branchs.Count == 0)
			{
				action(self);
				return self;
			}

			INode current;
			UnitStack<INode> stack = self.Core.PoolGetUnit<UnitStack<INode>>();
			UnitStack<INode> allStack = self.Core.PoolGetUnit<UnitStack<INode>>();
			stack.Push(self);
			while (stack.Count != 0)
			{
				current = stack.Pop();
				allStack.Push(current);
				if (current.m_Branchs != null)
				{
					foreach (var branchs in current.m_Branchs)
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
		public static INode TraversalPrePostOrder(INode self, Action<INode> PreAction, Action<INode> PostAction)
		{
			//自己没有分支
			if (self.m_Branchs == null || self.m_Branchs.Count == 0)
			{
				PreAction(self);
				PostAction(self);
				return self;
			}

			INode current;
			UnitStack<INode> stack = self.Core.PoolGetUnit<UnitStack<INode>>();
			UnitStack<INode> allStack = self.Core.PoolGetUnit<UnitStack<INode>>();
			stack.Push(self);
			while (stack.Count != 0)
			{
				current = stack.Pop();
				PreAction(current);
				allStack.Push(current);
				if (current.m_Branchs != null)
				{
					foreach (var branchs in current.m_Branchs)
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
				PostAction(allStack.Pop());
			}
			allStack.Dispose();
			return self;
		}
	}
}
