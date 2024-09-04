/****************************************

* 作者：闪电黑客
* 日期：2024/8/20 17:35

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 树数据节点
	/// </summary>
	public class TreeData : Node
		, StringNodeOf<TreeData>
		, TempOf<INode>
		, AsAwake
	{
		/// <summary>
		/// 类型名称
		/// </summary>
		public string TypeName;
	}

	/// <summary>
	/// 树数值
	/// </summary>
	public class TreeValue : TreeData
	{
		/// <summary>
		/// 数据
		/// </summary>
		public object Value;
	}
}
