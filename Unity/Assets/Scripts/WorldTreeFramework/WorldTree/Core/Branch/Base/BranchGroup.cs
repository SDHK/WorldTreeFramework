/****************************************

* 作者： 闪电黑客
* 日期： 2025/9/18 17:53

* 描述： 

*/
using System.Collections.Generic;
namespace WorldTree
{
	/// <summary>
	/// 分支集合
	/// </summary>
	[TreeDataSerializable]
	public partial class BranchGroup : UnitDictionary<long, IBranch>, IBranchBase, ISerializable
	{
		[TreeDataIgnore]
		public int BranchCount => Count;

		public void OnDeserialize()
		{
			//清除值为空的分支
			UnitList<long> keyList = Core.PoolGetUnit<UnitList<long>>();
			foreach (var item in this) if (item.Value == null) keyList.Add(item.Key);
			foreach (var key in keyList) Remove(key);
		}

		public void OnSerialize()
		{
		}


		public bool ContainsBranch(long typeCode) => ContainsKey(typeCode);

		public IBranch GetBranch(long typeCode) => TryGetValue(typeCode, out IBranch branch) ? branch : null;

		public bool TryGetBranch(long typeCode, out IBranch branch) => TryGetValue(typeCode, out branch);

		IEnumerator<IBranch> IEnumerable<IBranch>.GetEnumerator() => this.Values.GetEnumerator();

		public bool TryAddBranch(long typeCode, IBranch branch)
		{
			if (branch == null) return false;
			return TryAdd(typeCode, branch);
		}

		public void RemoveBranch(long typeCode) => Remove(typeCode);
	}
}