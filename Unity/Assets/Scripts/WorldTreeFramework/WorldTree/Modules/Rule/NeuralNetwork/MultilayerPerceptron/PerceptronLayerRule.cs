﻿/****************************************

* 作者：闪电黑客
* 日期：2024/12/31 15:47

* 描述：

*/
namespace WorldTree
{
    public static partial class PerceptronLayerRule
    {
        class PerceptronLayerAwakeRule : AwakeRule<PerceptronLayer, int>
        {
            protected override void Execute(PerceptronLayer self, int count)
            {
                self.AddChild(out self.NodeList);

                for (int i = 0; i < count; i++)
                {
                    self.NodeList.Add(self.AddChild(out PerceptronNode _));
                }
            }
        }

        class PerceptronLayerRemoveRule : RemoveRule<PerceptronLayer>
        {
            protected override void Execute(PerceptronLayer self)
            {
                self.NodeList = null;
            }
        }
    }
}
