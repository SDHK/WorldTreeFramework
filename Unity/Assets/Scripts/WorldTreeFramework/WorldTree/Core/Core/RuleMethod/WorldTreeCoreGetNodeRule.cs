
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
			=> node = self.NewNode(typeof(T), out _) as T;

		/// <summary>
		/// 新建节点对象
		/// </summary>
		public static INode NewNode(this WorldTreeCore self, Type type, out INode node)
		{
			node = Activator.CreateInstance(type, true) as INode;
			node.Core = self;
			node.Root = self.Root;
			node.Type = node.TypeToCode(type);
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
			=> self.PoolGetNode(self.TypeToCode<T>()) as T;

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
			return self.NewNode(self.CodeToType(type), out _);
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
