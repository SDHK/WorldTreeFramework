/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// 感知器连线
	/// </summary>
	public partial class PerceptronLine : Node, ChildOf<PerceptronNode>
		, AsRule<Awake<PerceptronNode, PerceptronNode>>
	{
		/// <summary>
		/// 随机数
		/// </summary>
		public static Random rand = new Random();

		/// <summary>
		/// 上连接
		/// </summary>
		public PerceptronNode Node1;

		/// <summary>
		/// 下连接
		/// </summary>
		public PerceptronNode Node2;

		/// <summary>
		/// 权重
		/// </summary>
		public double Weight = 0;

		public override string ToString()
		{
			return $"{this.GetType().Name}\tweight:[{Weight}]";
		}



		[NodeRule(nameof(AwakeRule<PerceptronLine, PerceptronNode, PerceptronNode>))]
		private static void OnAwakeRule(PerceptronLine self, PerceptronNode node1, PerceptronNode node2)
		{
			self.Node1 = node1;
			self.Node2 = node2;
			self.Weight = PerceptronLine.rand.NextDouble() * 2.0 - 1.0;
		}

		[NodeRule(nameof(RemoveRule<PerceptronLine>))]
		private static void OnRemoveRule(PerceptronLine self)
		{
			self.Weight = 0;
			self.Node1 = null;
			self.Node2 = null;
		}
		/// <summary>
		/// 权重变更
		/// </summary>
		public void BackPropagationWeight()
		{
			if (this.Node1 is null || this.Node2 is null) return;

			//权重+=上连接的结果*下连接的误差增量
			this.Weight += this.Node1.Result * this.Node2.Delta;
		}
	}

}
