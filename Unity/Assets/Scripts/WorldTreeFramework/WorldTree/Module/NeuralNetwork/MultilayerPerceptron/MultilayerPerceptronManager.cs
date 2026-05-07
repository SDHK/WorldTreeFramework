/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 多层感知器管理器
	/// </summary>
	public partial class MultilayerPerceptronManager : Node, ChildOf<INode>, ComponentOf<INode>
		, AsComponentBranch
		, AsChildBranch
		, AsRule<Awake>
	{
		/// <summary>
		/// 感知器层
		/// </summary>
		public TreeList<PerceptronLayer> LayerList;

		[NodeRule(nameof(AddRule<MultilayerPerceptronManager>))]
		private static void OnAddRule(MultilayerPerceptronManager self)
		{
			self.AddComponent(out self.LayerList);
		}

		[NodeRule(nameof(RemoveRule<MultilayerPerceptronManager>))]
		private static void OnRemoveRule(MultilayerPerceptronManager self)
		{
			self.LayerList = null;
		}


		/// <summary>
		/// 添加一层
		/// </summary>
		public void AddLayer(int count)
		{
			this.LayerList.Add(this.AddChild(out PerceptronLayer currentLayer, count));
			if (this.LayerList.Count > 1)
			{
				var parentNodeList = this.LayerList[^2].NodeList;
				var childNodeList = currentLayer.NodeList;

				foreach (var parentNode in parentNodeList)
				{
					foreach (var childNode in childNodeList)
					{
						parentNode.Link(childNode);
					}
				}
			}
		}

		/// <summary>
		/// 输入并正向传播
		/// </summary>
		public MultilayerPerceptronManager SetInputs(params double[] values)
		{
			if (this.LayerList.Count < 2)
			{
				this.LogError("网络层数小于2");
			}
			if (this.LayerList[0].NodeList.Count != values.Length)
			{
				this.LogError("参数数量与第输入层节点数不等");
			}

			for (int i = 0; i < values.Length; i++)
			{
				this.LayerList[0].NodeList[i].Result = values[i];
			}

			this.ForwardPropagation();
			return this;
		}

		/// <summary>
		/// 纠正输出并反向传播
		/// </summary>
		public void SetOutputs(params double[] values)
		{
			if (this.LayerList.Count < 2)
			{
				this.LogError("网络层数小于2");
			}
			if (this.LayerList[^1].NodeList.Count != values.Length)
			{
				this.LogError("参数数量与输出层节点数不等");
			}

			for (int i = 0; i < values.Length; i++)
			{
				var node = this.LayerList[^1].NodeList[i];
				node.SetError(values[i] - node.Result);
			}

			this.BackPropagation();
		}

		/// <summary>
		/// 获取最终输出
		/// </summary>
		public double[] GetOutputs()
		{
			var nodeList = this.LayerList[^1].NodeList;
			double[] outputs = new double[nodeList.Count];
			for (int i = 0; i < nodeList.Count; i++)
			{
				outputs[i] = nodeList[i].Result;
			}
			return outputs;
		}

		/// <summary>
		/// 正向传播
		/// </summary>
		public void ForwardPropagation()
		{
			for (int x = 1; x < this.LayerList.Count; x++)
			{
				for (int y = 0; y < this.LayerList[x].NodeList.Count; y++)
				{
					this.LayerList[x].NodeList[y].ForwardPropagation();
				}
			}
		}

		/// <summary>
		/// 反向传播
		/// </summary>
		public void BackPropagation()
		{
			for (int x = this.LayerList.Count - 2; x >= 0; x--)
			{
				for (int y = 0; y < this.LayerList[x].NodeList.Count; y++)
				{
					this.LayerList[x].NodeList[y].BackPropagation();
				}
			}
		}
	}
}
