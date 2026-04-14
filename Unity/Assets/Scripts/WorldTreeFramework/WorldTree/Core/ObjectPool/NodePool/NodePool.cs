/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/17 17:23

* 描述： 树节点对象池
*
*/
using System;

namespace WorldTree
{
	/// <summary>
	/// 实体对象池
	/// </summary>
	public class NodePool : PoolBase
	{
		public override string ToString()
		{
			return $"[NodePool<{ObjectType}>] : {Count} ";
		}

		protected override object NewObject()
		{
			INode obj = Activator.CreateInstance(ObjectType, true) as INode;
			obj.IsFromPool = true;
			obj.Type = ObjectTypeCode;
			return obj;
		}

		public override void Recycle(object obj)
		{
			if (obj is not INode node) return;
			if (node.IsDisposed) return;
			node.IsDisposed = true;
			node.Id = 0;
			node.InstanceId = 0;
			node.IsSerialize = false;
			node.IsActive = false;
			node.IsDisposed = false;
			base.Recycle(obj);
		}

		public override object GetObject()
		{
			INode obj = base.GetObject() as INode;
			obj.IsDisposed = false;
			return obj;
		}
	}
}
