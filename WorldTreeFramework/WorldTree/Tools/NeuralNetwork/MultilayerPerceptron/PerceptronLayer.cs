﻿/****************************************

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
        , AsRule<IAwakeRule<int>>
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

