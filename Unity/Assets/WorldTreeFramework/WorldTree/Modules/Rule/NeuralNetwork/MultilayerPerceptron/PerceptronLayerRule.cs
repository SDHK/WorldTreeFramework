/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/25 20:45

* 描述： 

*/

namespace WorldTree
{
    public static partial class PerceptronLayerRule
    {
        class PerceptronLayerAwakeRule : AwakeRule<PerceptronLayer, int>
        {
            protected override void OnEvent(PerceptronLayer self, int count)
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
            protected override void OnEvent(PerceptronLayer self)
            {
                self.nodes = null;
            }
        }
    }
}
