namespace WorldTree
{

    public static partial class MultilayerPerceptronManagerRule
    {
        class AddRule : AddRule<MultilayerPerceptronManager>
        {
            public override void OnEvent(MultilayerPerceptronManager self)
            {
                self.AddComponent(out self.layers);
            }
        }

        class RemoveRule : RemoveRule<MultilayerPerceptronManager>
        {
            public override void OnEvent(MultilayerPerceptronManager self)
            {
                self.layers = null;
            }
        }


        /// <summary>
        /// 添加一层
        /// </summary>
        public static void AddLayer(this MultilayerPerceptronManager self, int count)
        {
            self.layers.Add(self.AddChild(out PerceptronLayer currentLayer, count));
            if (self.layers.Count > 1)
            {
                var parentNodes = self.layers[^2].nodes;
                var childNodes = currentLayer.nodes;

                foreach (var parentNode in parentNodes)
                {
                    foreach (var childNode in childNodes)
                    {
                        parentNode.Link(childNode);
                    }
                }
            }
        }

        /// <summary>
        /// 输入并正向传播
        /// </summary>
        public static MultilayerPerceptronManager SetInputs(this MultilayerPerceptronManager self, params double[] values)
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
            return self;
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
            if (self.layers[^1].nodes.Count != values.Length)
            {
                World.LogError("参数数量与输出层节点数不等");
            }

            for (int i = 0; i < values.Length; i++)
            {
                var node = self.layers[^1].nodes[i];
                node.SetError(values[i] - node.result);
            }

            self.BackPropagation();
        }

        /// <summary>
        /// 获取最终输出
        /// </summary>
        public static double[] GetOutputs(this MultilayerPerceptronManager self)
        {
            var nodes = self.layers[^1].nodes;
            double[] outputs = new double[nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
            {
                outputs[i] = nodes[i].result;
            }
            return outputs;
        }

        /// <summary>
        /// 正向传播
        /// </summary>
        public static void ForwardPropagation(this MultilayerPerceptronManager self)
        {
            for (int x = 1; x < self.layers.Count; x++)
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
            for (int x = self.layers.Count - 2; x >= 0; x--)
            {
                for (int y = 0; y < self.layers[x].nodes.Count; y++)
                {
                    self.layers[x].nodes[y].BackPropagation();
                }
            }
        }
    }

}
