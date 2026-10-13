
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/11 10:55

* 描述： 单位对象池
* 

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 单位对象池
	/// </summary>
	public class UnitPool : PoolBase
	{
		public override string ToString()
		{
			return $"[UnitPool<{ObjectType}>] : {Count} ";
		}

		public override void Recycle(object obj)
		{
			if (obj is not IUnit unit) return;
			if (unit.IsDisposed) return;
			unit.IsDisposed = true;
			unit.OnDispose();
			base.Recycle(obj);
		}

		protected override object NewObject()
		{
			IUnit obj = Activator.CreateInstance(ObjectType, true) as IUnit;
			obj.Type = ObjectTypeCode;
			obj.IsFromPool = true;
			return obj;
		}

		public override object GetObject()
		{
			IUnit obj = base.GetObject() as IUnit;
			obj.IsDisposed = false;
			return obj;
		}
	}
}
