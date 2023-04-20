
/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/20 10:26

* 描述： 多层感知机管理器

*/


namespace WorldTree
{
    /// <summary>
    /// 多层感知机管理器
    /// </summary>
    public class MultilayerPerceptronManager : Node, IAwake, ChildOf<INode>, ComponentOf<INode>
    {
        public TreeList<PerceptronLayer> layers;
    }


    class MultilayerPerceptronManagerAddRule : AddRule<MultilayerPerceptronManager>
    {
        public override void OnEvent(MultilayerPerceptronManager self)
        {
            self.AddComponent(out self.layers);
        }
    }

    class MultilayerPerceptronManagerRemoveRule : RemoveRule<MultilayerPerceptronManager>
    {
        public override void OnEvent(MultilayerPerceptronManager self)
        {
            self.layers = null;
        }
    }


    public static class MultilayerPerceptronManagerRule
    {
        /// <summary>
        /// 添加一层
        /// </summary>
        public static void AddLayer(this MultilayerPerceptronManager self, int count)
        {
            self.layers.Add(self.AddChild(out PerceptronLayer currentLayer, count));
            if (self.layers.Count > 1)
            {
                var childNodes = currentLayer.nodes;
                var parentNodes = self.layers[self.layers.Count - 2].nodes;

                foreach (var item in parentNodes)
                {
                    item.List2 = childNodes;
                }
                foreach (var item in childNodes)
                {
                    item.List1 = parentNodes;
                }
            }
        }

        /// <summary>
        /// 输入并正向传播
        /// </summary>
        public static void SetInputs(this MultilayerPerceptronManager self, params double[] values)
        {
            if (self.layers.Count < 2)
            {
                World.LogError("网络层数小于2");
            }
            if (self.layers[0].nodes.Count != values.Length)
            {
                World.LogError("参数数量与第输入层节点数不等");
            }

            for (int i = 0; i < values.Length; i++)
            {
                self.layers[0].nodes[i].result = values[i];
            }

            self.ForwardPropagation();
        }

        /// <summary>
        /// 纠正输出并反向传播
        /// </summary>
        public static void SetOutputs(this MultilayerPerceptronManager self, params double[] values)
        {
            if (self.layers.Count < 2)
            {
                World.LogError("网络层数小于2");
            }
            if (self.layers[self.layers.Count - 1].nodes.Count != values.Length)
            {
                World.LogError("参数数量与输出层节点数不等");
            }

            for (int i = 0; i < values.Length; i++)
            {
                self.layers[self.layers.Count - 1].nodes[i].weight = values[i];
            }

            self.BackPropagation();
        }

        /// <summary>
        /// 正向传播
        /// </summary>
        public static void ForwardPropagation(this MultilayerPerceptronManager self)
        {
            for (int x = 0; x < self.layers.Count; x++)
            {
                for (int y = 0; y < self.layers[x].nodes.Count; y++)
                {
                    self.layers[x].nodes[y].ForwardPropagation();
                }
            }
        }

        /// <summary>
        /// 反向传播
        /// </summary>
        public static void BackPropagation(this MultilayerPerceptronManager self)
        {
            for (int x = self.layers.Count - 1; x >= 0; x--)
            {
                for (int y = 0; y < self.layers[x].nodes.Count; y++)
                {
                    self.layers[x].nodes[y].BackPropagation();
                }
            }

            for (int x = self.layers.Count - 1; x >= 0; x--)
            {
                for (int y = 0; y < self.layers[x].nodes.Count; y++)
                {
                    self.layers[x].nodes[y].BackPropagationWeight();
                }
            }
        }
    }
}
