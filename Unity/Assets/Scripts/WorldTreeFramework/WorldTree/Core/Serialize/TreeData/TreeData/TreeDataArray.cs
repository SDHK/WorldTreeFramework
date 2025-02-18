/****************************************

* 作者：闪电黑客
* 日期：2024/9/3 12:08

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 树数据数组节点
	/// </summary>
	public class TreeDataArray : TreeData
	{
		/// <summary>
		/// 维度长度列表
		/// </summary>
		public UnitList<int> LengthList;

		public override string ToString()
		{
			return $"[TreeArray:{this.TypeName}]:[{string.Join(",", LengthList)}]";
		}
	}

	public static class TreeDataArrayRule
	{
		class Add : AddRule<TreeDataArray>
		{
			protected override void Execute(TreeDataArray self)
			{
				self.Core.PoolGetUnit(out self.LengthList);
			}
		}

		class Remove : RemoveRule<TreeDataArray>
		{
			protected override void Execute(TreeDataArray self)
			{
				self.LengthList.Dispose();
				self.LengthList = null;
			}
		}
	}
}
