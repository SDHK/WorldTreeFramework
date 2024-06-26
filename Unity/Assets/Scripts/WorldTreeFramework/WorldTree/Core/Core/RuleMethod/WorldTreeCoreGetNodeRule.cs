﻿
/****************************************

* 作者： 闪电黑客
* 日期： 2023/9/11 20:42

* 描述： 

*/

using System;

namespace WorldTree
{
	public static partial class WorldTreeCoreRule
	{
		/// <summary>
		/// 新建节点对象
		/// </summary>
		/// <remarks>不执行法则生命周期</remarks>
		public static T NewNode<T>(this WorldTreeCore self, out T node) where T : class, INode
		{
			Type type = typeof(T);
			node = Activator.CreateInstance(type, true) as T;
			node.Type = TypeInfo<T>.TypeCode;
			node.Core = self;
			node.Root = self.Root;
			node.Id = self.IdManager.GetId();
			return node;
		}

		/// <summary>
		/// 新建节点对象
		/// </summary>
		/// <remarks>不执行法则生命周期</remarks>
		public static INode NewNode(this WorldTreeCore self, long type)
		{
			INode node = Activator.CreateInstance(type.CodeToType(), true) as INode;
			node.Type = type;
			node.Core = self;
			node.Root = self.Root;
			node.Id = self.IdManager.GetId();
			return node;
		}

		/// <summary>
		/// 新建节点对象并调用生命周期
		/// </summary>
		/// <remarks>执行法则生命周期</remarks>
		public static T NewNodeLifecycle<T>(this WorldTreeCore self, out T node) where T : class, INode
		{
			self.NewNode(out node);
			self.RuleManager.SupportNodeRule(node.Type);
			self.NewRuleGroup?.Send(node);
			self.GetRuleGroup?.Send(node);
			return node;
		}

		/// <summary>
		/// 新建节点对象并调用生命周期
		/// </summary>
		/// <remarks>执行法则生命周期</remarks>
		public static INode NewNodeLifecycle(this WorldTreeCore self, long type)
		{
			INode node = self.NewNode(type);
			self.RuleManager.SupportNodeRule(node.Type);
			self.NewRuleGroup?.Send(node);
			self.GetRuleGroup?.Send(node);
			return node;
		}


		/// <summary>
		/// 从池中获取节点对象
		/// </summary>
		public static T PoolGetNode<T>(this WorldTreeCore self) where T : class, INode
		{
			if (self.IsCoreActive)
			{
				if (self.NodePoolManager.TryGet(TypeInfo<T>.TypeCode, out INode node))
				{
					node.Id = self.IdManager.GetId();
					return node as T;
				}
			}
			return self.NewNodeLifecycle<T>(out _);
		}

		/// <summary>
		/// 从池中获取节点对象
		/// </summary>
		public static INode PoolGetNode(this WorldTreeCore self, long type)
		{
			if (self.IsCoreActive)
			{
				if (self.NodePoolManager.TryGet(type, out INode node))
				{
					node.Id = self.IdManager.GetId();
					return node;
				}
			}
			return self.NewNodeLifecycle(type);
		}

		/// <summary>
		/// 回收节点
		/// </summary>
		public static void PoolRecycle(this WorldTreeCore self, INode obj)
		{
			if (self.IsCoreActive && obj.IsFromPool)
			{
				if (self.NodePoolManager.TryRecycle(obj)) return;
			}
			obj.IsDisposed = true;
			self.RecycleRuleGroup?.Send(obj);
			self.DestroyRuleGroup?.Send(obj);
			obj.Id = 0;
		}
	}
}
