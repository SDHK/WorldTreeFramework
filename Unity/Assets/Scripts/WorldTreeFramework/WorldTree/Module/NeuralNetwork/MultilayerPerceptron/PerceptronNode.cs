/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// 感知器单元节点
	/// </summary>
	public partial class PerceptronNode : Node, ChildOf<INode>
		, AsChildBranch
		, AsRule<Awake>
	{
		/// <summary>
		/// 上连接
		/// </summary>
		public TreeList<PerceptronLine> Link1List;

		/// <summary>
		/// 下连接
		/// </summary>
		public TreeList<PerceptronLine> Link2List;

		/// <summary>
		/// 误差增量
		/// </summary>
		public double Delta = 0;

		/// <summary>
		/// 偏置项
		/// </summary>
		public double Bias = 0;

		/// <summary>
		/// 结果
		/// </summary>
		public double Result = 0;

		//public override string ToString()
		//{
		//    return $"{this.GetType().Name}\tDelta:[{Delta}]\tBias:[{Bias}]\tResult:[{Result}]";
		//}

		[NodeRule(nameof(RemoveRule<PerceptronNode>))]
		private static void OnRemoveRule(PerceptronNode self)
		{
			self.Result = 0;
			self.Delta = 0;
			self.Link1List = null;
			self.Link2List = null;
		}

		/// <summary>
		/// 连接
		/// </summary>
		public void Link(PerceptronNode node)
		{
			_ = this.Link2List ?? this.AddChild(out this.Link2List);
			_ = node.Link1List ?? node.AddChild(out node.Link1List);

			this.AddChild(out PerceptronLine perceptronLine, this, node);

			this.Link2List.Add(perceptronLine);
			node.Link1List.Add(perceptronLine);
		}


		/// <summary>
		/// 正向传播计算
		/// </summary>
		public void ForwardPropagation()
		{
			if (this.Link1List != null)
			{
				double thresholdResults = this.Bias;
				for (int i = 0; i < this.Link1List.Count; i++)
				{
					thresholdResults = (this.Link1List[i].Node1.Result * this.Link1List[i].Weight) + thresholdResults;
				}
				//通过 激活函数 拿到 0 到 1 的数值
				this.Result = 1d / (Math.Exp(-thresholdResults) + 1);
			}
		}

		/// <summary>
		/// 反向传播计算
		/// </summary>
		public void BackPropagation()
		{
			if (this.Link2List != null)
			{
				double error = 0;
				for (int i = 0; i < this.Link2List.Count; i++)
				{
					//误差 += 下级节点的 误差增量 * 权重 
					error += this.Link2List[i].Node2.Delta * this.Link2List[i].Weight;
					this.Link2List[i].BackPropagationWeight();
				}
				this.SetError(error);
			}
		}

		/// <summary>
		/// 设置误差
		/// </summary>
		public void SetError(double error)
		{
			//误差增量 = 斜率 * 误差。 斜率就是向谷底接近的速度
			this.Delta = (this.Result * (1d - this.Result)) * error;
			this.Bias += this.Delta;
		}

	}
}
