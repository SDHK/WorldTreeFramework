/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// 多层感知机测试
	/// </summary>
	public partial class MultilayerPerceptronTest : Node, ComponentOf<InitialDomain>
		, AsComponentBranch
		, AsRule<Awake>
	{
		/// <summary>
		/// 多层感知机管理器
		/// </summary>
		public MultilayerPerceptronManager multilayerPerceptronManager;

		[NodeRule(nameof(AwakeRule<MultilayerPerceptronTest>))]
		private static void OnAwakeRule(MultilayerPerceptronTest self)
		{
			self.AddComponent(out self.multilayerPerceptronManager);
			self.multilayerPerceptronManager.AddLayer(3);
			self.multilayerPerceptronManager.AddLayer(4);
			self.multilayerPerceptronManager.AddLayer(1);
		}

		[NodeRule(nameof(UpdateRule<MultilayerPerceptronTest>))]
		private static void OnUpdateRule(MultilayerPerceptronTest self)
		{
			if (Input.GetKeyDown(KeyCode.A))
			{
				for (int i = 0; i < 1; i++)
				{
					self.Exercise();
				}
			}
			if (Input.GetKeyDown(KeyCode.S))
			{
				for (int i = 0; i < 10; i++)
				{
					self.Exercise();
				}
			}

			if (Input.GetKeyDown(KeyCode.D))
			{
				for (int i = 0; i < 100; i++)
				{
					self.Exercise();
				}
			}
			if (Input.GetKeyDown(KeyCode.F))
			{
				for (int i = 0; i < 1000; i++)
				{
					self.Exercise();
				}
			}

			if (Input.GetKeyDown(KeyCode.Z))
			{
				self.multilayerPerceptronManager.SetInputs(1, 1, 1);
				self.Log($"结果 {self.multilayerPerceptronManager.LayerList[self.multilayerPerceptronManager.LayerList.Count - 1].NodeList[0].Result}");
			}

			if (Input.GetKeyDown(KeyCode.X))
			{
				self.multilayerPerceptronManager.SetInputs(1, 0, 1);
				self.Log($"结果 {self.multilayerPerceptronManager.LayerList[self.multilayerPerceptronManager.LayerList.Count - 1].NodeList[0].Result}");
			}

			if (Input.GetKeyDown(KeyCode.C))
			{
				self.multilayerPerceptronManager.SetInputs(0, 1, 1);
				self.Log($"结果 {self.multilayerPerceptronManager.LayerList[self.multilayerPerceptronManager.LayerList.Count - 1].NodeList[0].Result}");
			}

			if (Input.GetKeyDown(KeyCode.V))
			{
				self.multilayerPerceptronManager.SetInputs(0, 0, 1);
				self.Log($"结果 {self.multilayerPerceptronManager.LayerList[self.multilayerPerceptronManager.LayerList.Count - 1].NodeList[0].Result}");
			}
		}


		/// <summary>
		/// 训练
		/// </summary>
		private void Exercise()
		{
			this.multilayerPerceptronManager.SetInputs(0, 0, 0);
			this.multilayerPerceptronManager.SetOutputs(0);

			this.multilayerPerceptronManager.SetInputs(0, 1, 0);
			this.multilayerPerceptronManager.SetOutputs(1);

			this.multilayerPerceptronManager.SetInputs(1, 0, 0);
			this.multilayerPerceptronManager.SetOutputs(1);

			this.multilayerPerceptronManager.SetInputs(1, 1, 0);
			this.multilayerPerceptronManager.SetOutputs(0);


			this.multilayerPerceptronManager.SetInputs(0, 0, 1);
			this.multilayerPerceptronManager.SetOutputs(0);

			this.multilayerPerceptronManager.SetInputs(0, 1, 1);
			this.multilayerPerceptronManager.SetOutputs(1);


			this.multilayerPerceptronManager.SetInputs(1, 0, 1);
			this.multilayerPerceptronManager.SetOutputs(1);

			this.multilayerPerceptronManager.SetInputs(1, 1, 1);
			this.multilayerPerceptronManager.SetOutputs(0);
		}

	}
}
