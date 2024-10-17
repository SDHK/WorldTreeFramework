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
	public class TreeData1 : Node
		, StringNodeOf<TreeData1>
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
	public class TreeValue : TreeData1
	{
		/// <summary>
		/// 数据
		/// </summary>
		public object Value;
	}
}
