/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/11 11:05

* 描述： 单位对象池管理器

*/

namespace WorldTree
{
	/// <summary>
	/// 单位对象池管理器
	/// </summary>
	public class UnitPoolManager : PoolManagerBase<UnitPool>
		, CoreManagerOf<WorldLine>
		, AsRule<Awake>
	{
		/// <summary>
		/// 尝试获取单位
		/// </summary>
		public bool TryGet(long type, out IUnit unit)
		{
			if (TryGet(type, out object obj))
			{
				unit = obj as IUnit;
				return true;
			}
			else
			{
				unit = null;
				return false;
			}
		}
	}
}
