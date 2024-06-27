/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/20 10:28

* 描述： 感知器层级

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

