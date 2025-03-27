/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/17 17:23

* 描述： 树节点对象池
*
* 管理类型： INode
*   
*   调用 节点 生命周期法则， 生成， 获取， 回收， 销毁，
*  
*   
*/
using System;

namespace WorldTree
{

	/// <summary>
	/// 实体对象池
	/// </summary>
	public class NodePool : GenericPool<INode>
		, ChildOf<PoolManagerBase<NodePool>>
	{
		public NodePool() : base()
		{
			NewObject = ObjectNew;
			objectOnGet = ObjectOnGet;
			objectOnRecycle = ObjectOnRecycle;
		}

		public override string ToString()
		{
			return $"[NodePool<{ObjectType}>] : {Count} ";
		}

		/// <summary>
		/// 获取对象并转为指定类型
		/// </summary>
		public T Get<T>()
			where T : class
		{
			return Get() as T;
		}

		public override void Recycle(object obj) => Recycle(obj as INode);
		/// <summary>
		/// 回收对象
		/// </summary>
		public void Recycle(INode obj)
		{
			lock (objectPoolQueue)
			{
				if (obj != null)
				{
					if (maxLimit == -1 || objectPoolQueue.Count < maxLimit)
					{
						if (obj.IsDisposed) return;

						objectOnRecycle.Invoke(obj);
						objectPoolQueue.Enqueue(obj);
					}
					else
					{
						objectOnRecycle.Invoke(obj);
					}
				}
			}
		}
		/// <summary>
		/// 对象新建方法
		/// </summary>
		private INode ObjectNew(IPool pool)
		{
			INode obj = Activator.CreateInstance(ObjectType, true) as INode;
			obj.Core = Core;
			obj.World = Core.World;
			obj.Type = ObjectTypeCode;
			return obj;
		}
		/// <summary>
		/// 对象获取处理事件
		/// </summary>
		private void ObjectOnGet(INode obj)
		{
			obj.IsFromPool = true;
			obj.IsSerialize = false;
			obj.IsActive = false;
			obj.IsDisposed = false;
		}
		/// <summary>
		/// 对象回收处理事件
		/// </summary>
		public void ObjectOnRecycle(INode obj)
		{
			obj.IsDisposed = true;
			obj.Id = 0;
			obj.InstanceId = 0;
		}
	}


	public static partial class NodePoolRule
	{
		class RemoveRule : RemoveRule<NodePool>
		{
			protected override void Execute(NodePool self)
			{
				self.DisposeAll();
				self.NewObject = null;
				self.objectOnGet = null;
				self.objectOnRecycle = null;
			}
		}
	}
}
