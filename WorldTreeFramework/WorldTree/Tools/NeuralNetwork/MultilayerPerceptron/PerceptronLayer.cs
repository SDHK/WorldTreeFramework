/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/20 10:28

* 描述： 感知机层级

*/



namespace WorldTree
{
    /// <summary>
    /// 感知机层级
    /// </summary>
    public class PerceptronLayer : Node, IAwake<int>, ChildOf<INode>
    {
        public TreeList<PerceptronNode> nodes;
    }

    class PerceptronLayerAwakeRule : AwakeRule<PerceptronLayer, int>
    {
        public override void OnEvent(PerceptronLayer self, int count)
        {
            self.AddComponent(out self.nodes);

            for (int i = 0; i < count; i++)
            {
                self.nodes.Add(self.AddChild(out PerceptronNode _));
            }
        }
    }

    class PerceptronLayerRemoveRule : RemoveRule<PerceptronLayer>
    {
        public override void OnEvent(PerceptronLayer self)
        {
            self.nodes = null;
        }
    }

}

