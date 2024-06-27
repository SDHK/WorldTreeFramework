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
            protected override void Execute(PerceptronLine self, PerceptronNode node1, PerceptronNode node2)
            {
                self.Node1 = node1;
                self.Node2 = node2;
                self.Weight = PerceptronLine.rand.NextDouble() * 2.0 - 1.0;
            }

        }
        class RemoveRule : RemoveRule<PerceptronLine>
        {
            protected override void Execute(PerceptronLine self)
            {
                self.Weight = 0;
                self.Node1 = null;
                self.Node2 = null;
            }
        }


        /// <summary>
        /// 权重变更
        /// </summary>
        public static void BackPropagationWeight(this PerceptronLine self)
        {
            if (self.Node1 is null || self.Node2 is null) return;

            //权重+=上连接的结果*下连接的误差增量
            self.Weight += self.Node1.Result * self.Node2.Delta;
        }
    }
}
