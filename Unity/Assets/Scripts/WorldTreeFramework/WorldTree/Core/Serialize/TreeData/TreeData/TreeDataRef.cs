/****************************************

* 作者：闪电黑客
* 日期：2024/9/3 12:08

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 树数据节点引用
	/// </summary>
	public class TreeDataRef : TreeData
	{
		/// <summary>
		/// 引用数据
		/// </summary>
		public TreeData Data;

		public override string ToString()
		{
			return $"[TreeDataRef]:{this.Data}";
		}
	}

	public static class TreeDataRefRule
	{
		class Remove : RemoveRule<TreeDataRef>
		{
			protected override void Execute(TreeDataRef self)
			{
				self.Data = null;
			}
		}
	}
}
