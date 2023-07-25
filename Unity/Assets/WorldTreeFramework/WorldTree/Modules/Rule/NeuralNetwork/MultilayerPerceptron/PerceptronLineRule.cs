/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/25 20:48

* 描述： 

*/

namespace WorldTree
{
    public static partial class PerceptronLineRule
    {
        class AwakeRule : AwakeRule<PerceptronLine, PerceptronNode, PerceptronNode>
        {
            public override void OnEvent(PerceptronLine self, PerceptronNode node1, PerceptronNode node2)
            {
                self.node1 = node1;
                self.node2 = node2;
                self.weight = PerceptronLine.rand.NextDouble() * 2.0 - 1.0;
            }

        }
        class RemoveRule : RemoveRule<PerceptronLine>
        {
            public override void OnEvent(PerceptronLine self)
            {
                self.weight = 0;
                self.node1 = null;
                self.node2 = null;
            }
        }


        /// <summary>
        /// 权重变更
        /// </summary>
        public static void BackPropagationWeight(this PerceptronLine self)
        {
            if (self.node1 is null || self.node2 is null) return;

            //权重+=上连接的结果*下连接的误差增量
            self.weight += self.node1.result * self.node2.delta;
        }
    }
}
