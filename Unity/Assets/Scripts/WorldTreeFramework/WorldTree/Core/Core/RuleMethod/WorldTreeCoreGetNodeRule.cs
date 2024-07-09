
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
		public static T NewNode<T>(this WorldTreeCore self, out T node) where T : class, INode
		=> node = self.NewNode(TypeInfo<T>.TypeCode) as T;

		/// <summary>
		/// 新建节点对象
		/// </summary>
		public static INode NewNode(this WorldTreeCore self, long type)
			=> self.NewNode(type,out _);

		/// <summary>
		/// 新建节点对象
		/// </summary>
		public static INode NewNode(this WorldTreeCore self, long type, out INode node)
		{
			node = Activator.CreateInstance(type.CodeToType(), true) as INode;
			node.Type = type;
			node.Core = self;
			node.Root = self.Root;
			node.OnCreate();
			return node;
		}


		/// <summary>
		/// 从池中获取节点对象
		/// </summary>
		public static T PoolGetNode<T>(this WorldTreeCore self, out T outT)
			where T : class, INode
		=> outT = self.PoolGetNode<T>();

		/// <summary>
		/// 从池中获取节点对象
		/// </summary>
		public static T PoolGetNode<T>(this WorldTreeCore self) where T : class, INode
			=> self.PoolGetNode(TypeInfo<T>.TypeCode) as T;

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
