
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
		/// 获取新建节点
		/// </summary>
		public static N GetOrNewNode<N>(this WorldTreeCore self, bool isPool = true) where N : class, INode => (isPool ? self.PoolGetNode(TypeInfo<N>.TypeCode) : self.NewNode(TypeInfo<N>.TypeCode)) as N;

		/// <summary>
		/// 获取新建节点
		/// </summary>
		public static INode GetOrNewNode(this WorldTreeCore self, long type, bool isPool = true) => (isPool ? self.PoolGetNode(type) : self.NewNode(type));

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
			node.OnCreate();
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
			node.OnCreate();
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
					node.OnCreate();
					return node as T;
				}
			}
			return self.NewNode<T>(out _);
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
					node.OnCreate();
					return node;
				}
			}
			return self.NewNode(type);
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
			obj.Id = 0;
		}
	}
}
