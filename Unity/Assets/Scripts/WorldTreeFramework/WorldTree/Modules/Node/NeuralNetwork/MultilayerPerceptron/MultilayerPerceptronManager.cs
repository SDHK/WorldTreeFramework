/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 多层感知器管理器
	/// </summary>
	public class MultilayerPerceptronManager : Node, ChildOf<INode>, ComponentOf<INode>
		, AsComponentBranch
		, AsChildBranch
		, AsAwake
	{
		/// <summary>
		/// 感知器层
		/// </summary>
		public TreeList<PerceptronLayer> LayerList;
	}
}
