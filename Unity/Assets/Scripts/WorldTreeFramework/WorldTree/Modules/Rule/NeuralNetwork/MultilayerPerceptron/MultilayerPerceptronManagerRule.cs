namespace WorldTree
{

    public static partial class MultilayerPerceptronManagerRule
    {
        class AddRule : AddRule<MultilayerPerceptronManager>
        {
            protected override void Execute(MultilayerPerceptronManager self)
            {
                self.AddComponent(out self.layerList);
            }
        }

        class RemoveRule : RemoveRule<MultilayerPerceptronManager>
        {
            protected override void Execute(MultilayerPerceptronManager self)
            {
                self.layerList = null;
            }
        }


        /// <summary>
        /// 添加一层
        /// </summary>
        public static void AddLayer(this MultilayerPerceptronManager self, int count)
        {
            self.layerList.Add(self.AddChild(out PerceptronLayer currentLayer, count));
            if (self.layerList.Count > 1)
            {
                var parentNodes = self.layerList[^2].NodeList;
                var childNodes = currentLayer.NodeList;

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
            if (self.layerList.Count < 2)
            {
                self.LogError("网络层数小于2");
            }
            if (self.layerList[0].NodeList.Count != values.Length)
            {
				self.LogError("参数数量与第输入层节点数不等");
            }

            for (int i = 0; i < values.Length; i++)
            {
                self.layerList[0].NodeList[i].Result = values[i];
            }

            self.ForwardPropagation();
            return self;
        }

        /// <summary>
        /// 纠正输出并反向传播
        /// </summary>
        public static void SetOutputs(this MultilayerPerceptronManager self, params double[] values)
        {
            if (self.layerList.Count < 2)
            {
				self.LogError("网络层数小于2");
            }
            if (self.layerList[^1].NodeList.Count != values.Length)
            {
				self.LogError("参数数量与输出层节点数不等");
            }

            for (int i = 0; i < values.Length; i++)
            {
                var node = self.layerList[^1].NodeList[i];
                node.SetError(values[i] - node.Result);
            }

            self.BackPropagation();
        }

        /// <summary>
        /// 获取最终输出
        /// </summary>
        public static double[] GetOutputs(this MultilayerPerceptronManager self)
        {
            var nodes = self.layerList[^1].NodeList;
            double[] outputs = new double[nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
            {
                outputs[i] = nodes[i].Result;
            }
            return outputs;
        }

        /// <summary>
        /// 正向传播
        /// </summary>
        public static void ForwardPropagation(this MultilayerPerceptronManager self)
        {
            for (int x = 1; x < self.layerList.Count; x++)
            {
                for (int y = 0; y < self.layerList[x].NodeList.Count; y++)
                {
                    self.layerList[x].NodeList[y].ForwardPropagation();
                }
            }
        }

        /// <summary>
        /// 反向传播
        /// </summary>
        public static void BackPropagation(this MultilayerPerceptronManager self)
        {
            for (int x = self.layerList.Count - 2; x >= 0; x--)
            {
                for (int y = 0; y < self.layerList[x].NodeList.Count; y++)
                {
                    self.layerList[x].NodeList[y].BackPropagation();
                }
            }
        }
    }

}
