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
	public partial class PerceptronLayer : Node, ChildOf<INode>
		, AsChildBranch
		, AsRule<Awake<int>>
	{
		/// <summary>
		/// 感知器节点
		/// </summary>
		public TreeList<PerceptronNode> NodeList;
		[NodeRule(nameof(AwakeRule<PerceptronLayer, int>))]
		private static void OnAwakeRule(PerceptronLayer self, int count)
		{
			self.AddChild(out self.NodeList);

			for (int i = 0; i < count; i++)
			{
				self.NodeList.Add(self.AddChild(out PerceptronNode _));
			}
		}

		[NodeRule(nameof(RemoveRule<PerceptronLayer>))]
		private static void OnRemoveRule(PerceptronLayer self)
		{
			self.NodeList = null;
		}
	}
}

