﻿/****************************************

* 作者：闪电黑客
* 日期：2024/9/3 12:08

* 描述：

*/
namespace WorldTree
{

	/// <summary>
	/// 树数据节点
	/// </summary>
	public class TreeData : Node
		, NumberNodeOf<TreeData>
		, ListNodeOf<TreeData>
		, ChildOf<TreeData>
		, TempOf<INode>
		, AsNumberNodeBranch
		, AsChildBranch
		, AsListNodeBranch
		, AsAwake
	{
		/// <summary>
		/// 类型名称
		/// </summary>
		public string TypeName;

		/// <summary>
		/// 是否默认值
		/// </summary>
		public bool IsDefault = false;

		public override string ToString()
		{
			return $"[TreeData:{this.TypeName}] {(IsDefault ? ": Null" : "")}";
		}
	}

	public static class TreeDataRule
	{
		class Remove : RemoveRule<TreeData>
		{
			protected override void Execute(TreeData self)
			{
				self.TypeName = null;
				self.IsDefault = false;
			}
		}
	}

}
