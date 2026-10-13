
/****************************************

* 作者： 闪电黑客
* 日期： 2023/9/11 20:42

* 描述： 

*/

using System;

namespace WorldTree
{
	public static partial class WorldRule
	{
		/// <summary>
		/// 新建节点对象
		/// </summary>
		public static T NewNode<T>(this World self, out T node, bool isSerialize = false) where T : class, INode
			=> node = self.NewNode(typeof(T), out _, isSerialize) as T;

		/// <summary>
		/// 新建节点对象
		/// </summary>
		public static INode NewNode(this World self, Type type, out INode node, bool isSerialize = false)
		{
			node = Activator.CreateInstance(type, true) as INode;
			node.World = self;
			node.World = self.World;
			node.Type = node.TypeToCode(type);
			node.IsSerialize = isSerialize;
			node.OnCreate();
			return node;
		}


		/// <summary>
		/// 从池中获取节点对象
		/// </summary>
		public static T PoolGetNode<T>(this World self, out T outT, bool isSerialize = false)
			where T : class, INode
		=> outT = self.PoolGetNode<T>(isSerialize);

		/// <summary>
		/// 从池中获取节点对象
		/// </summary>
		public static T PoolGetNode<T>(this World self, bool isSerialize = false) where T : class, INode
		{
			if (self.Line.Core.NodePoolManager.TryGet(out T node))
			{
				node.World = self;
				node.World = self.World;
				node.IsSerialize = isSerialize;
				node.OnCreate();
				return node;
			}
			return self.NewNode<T>(out _, isSerialize);
		}

		/// <summary>
		/// 从池中获取节点对象
		/// </summary>
		public static INode PoolGetNode(this World self, Type type, bool isSerialize = false)
		{
			if (self.Line.Core.NodePoolManager.TryGet(type, out object nodeObj))
			{
				INode node = nodeObj as INode;
				node.World = self;
				node.World = self.World;
				node.IsSerialize = isSerialize;
				node.OnCreate();
				return node;
			}
			return self.NewNode(type, out _, isSerialize);
		}

		/// <summary>
		/// 回收节点
		/// </summary>
		public static void PoolRecycle(this World self, INode obj)
		{
			if (obj.IsFromPool)
			{
				if (self.Line.Core.NodePoolManager.TryRecycle(obj)) return;
			}
			obj.IsDisposed = true;
			obj.Id = 0;
			obj.InstanceId = 0;
		}
	}
}
