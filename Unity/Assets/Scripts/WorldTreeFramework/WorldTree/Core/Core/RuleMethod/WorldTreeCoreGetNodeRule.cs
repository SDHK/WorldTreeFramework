
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
		public static T NewNode<T>(this WorldTreeCore self, out T node, bool isSerialize = false) where T : class, INode
			=> node = self.NewNode(typeof(T), out _, isSerialize) as T;

		/// <summary>
		/// 新建节点对象
		/// </summary>
		public static INode NewNode(this WorldTreeCore self, Type type, out INode node, bool isSerialize = false)
		{
			node = Activator.CreateInstance(type, true) as INode;
			node.Core = self;
			node.World = self.World;
			node.Type = node.TypeToCode(type);
			node.IsSerialize = isSerialize;
			node.OnCreate();
			return node;
		}


		/// <summary>
		/// 从池中获取节点对象
		/// </summary>
		public static T PoolGetNode<T>(this WorldTreeCore self, out T outT, bool isSerialize = false)
			where T : class, INode
		=> outT = self.PoolGetNode<T>(isSerialize);

		/// <summary>
		/// 从池中获取节点对象
		/// </summary>
		public static T PoolGetNode<T>(this WorldTreeCore self, bool isSerialize = false) where T : class, INode
			=> self.PoolGetNode(self.TypeToCode<T>(), isSerialize) as T;

		/// <summary>
		/// 从池中获取节点对象
		/// </summary>
		public static INode PoolGetNode(this WorldTreeCore self, long type, bool isSerialize = false)
		{
			if (self.IsCoreActive)
			{
				if (self.NodePoolManager.TryGet(type, out INode node))
				{
					node.IsSerialize = isSerialize;
					node.OnCreate();
					return node;
				}
			}
			return self.NewNode(self.CodeToType(type), out _, isSerialize);
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
			obj.InstanceId = 0;
		}
	}
}
