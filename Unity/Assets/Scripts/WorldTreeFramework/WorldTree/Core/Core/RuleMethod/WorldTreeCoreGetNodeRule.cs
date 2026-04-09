
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
		public static T NewNode<T>(this WorldLine self, out T node, bool isSerialize = false) where T : class, INode
			=> node = self.NewNode(typeof(T), out _, isSerialize) as T;

		/// <summary>
		/// 新建节点对象
		/// </summary>
		public static INode NewNode(this WorldLine self, Type type, out INode node, bool isSerialize = false)
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
		public static T PoolGetNode<T>(this WorldLine self, out T outT, bool isSerialize = false)
			where T : class, INode
		=> outT = self.PoolGetNode<T>(isSerialize);

		/// <summary>
		/// 从池中获取节点对象
		/// </summary>
		public static T PoolGetNode<T>(this WorldLine self, bool isSerialize = false) where T : class, INode
		{
			if (self.IsCoreActive)
			{
				lock (self.NodePoolManager)
				{
					if (self.NodePoolManager.TryGet(out T node))
					{
						node.IsSerialize = isSerialize;
						node.OnCreate();
						return node;
					}
				}
			}
			return self.NewNode<T>(out _, isSerialize);
		}

		/// <summary>
		/// 从池中获取节点对象
		/// </summary>
		public static INode PoolGetNode(this WorldLine self, Type type, bool isSerialize = false)
		{
			if (self.IsCoreActive)
			{
				lock (self.NodePoolManager)
				{
					if (self.NodePoolManager.TryGet(type, out object nodeObj))
					{
						INode node = nodeObj as INode;
						node.IsSerialize = isSerialize;
						node.OnCreate();
						return node;
					}
				}
			}
			return self.NewNode(type, out _, isSerialize);
		}

		/// <summary>
		/// 回收节点
		/// </summary>
		public static void PoolRecycle(this WorldLine self, INode obj)
		{
			if (self.IsCoreActive && obj.IsFromPool)
			{
				lock (self.NodePoolManager)
				{
					if (self.NodePoolManager.TryRecycle(obj)) return;
				}
			}
			obj.IsDisposed = true;
			obj.Id = 0;
			obj.InstanceId = 0;
		}
	}
}
