/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/20 10:26

* 描述： 多层感知器管理器
* 
*/

namespace WorldTree
{
    /// <summary>
    /// 多层感知器管理器
    /// </summary>
    public class MultilayerPerceptronManager : Node, ChildOf<INode>, ComponentOf<INode>
        ,AsRule<IAwakeRule>
    {
        public TreeList<PerceptronLayer> layers;
    }
}
