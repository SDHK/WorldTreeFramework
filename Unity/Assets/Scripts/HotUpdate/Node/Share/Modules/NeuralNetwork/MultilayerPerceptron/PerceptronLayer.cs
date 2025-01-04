/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 感知器层级
	/// </summary>
	public class PerceptronLayer : Node, ChildOf<INode>
		, AsChildBranch
		, AsAwake<int>
	{
		/// <summary>
		/// 感知器节点
		/// </summary>
		public TreeList<PerceptronNode> NodeList;
	}
}

