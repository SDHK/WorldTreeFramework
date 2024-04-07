using UnityEngine;

namespace WorldTree
{
	public static class MultilayerPerceptronTestRule
	{
		class AwakeRule : AwakeRule<MultilayerPerceptronTest>
		{
			protected override void Execute(MultilayerPerceptronTest self)
			{
				self.AddComponent(out self.multilayerPerceptronManager);
				self.multilayerPerceptronManager.AddLayer(3);
				self.multilayerPerceptronManager.AddLayer(4);
				self.multilayerPerceptronManager.AddLayer(1);
			}
		}

		class UpdateRule : UpdateRule<MultilayerPerceptronTest>
		{
			protected override void Execute(MultilayerPerceptronTest self)
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
					self.Log($"结果 {self.multilayerPerceptronManager.layers[self.multilayerPerceptronManager.layers.Count - 1].nodes[0].result}");
				}

				if (Input.GetKeyDown(KeyCode.X))
				{
					self.multilayerPerceptronManager.SetInputs(1, 0, 1);
					self.Log($"结果 {self.multilayerPerceptronManager.layers[self.multilayerPerceptronManager.layers.Count - 1].nodes[0].result}");
				}

				if (Input.GetKeyDown(KeyCode.C))
				{
					self.multilayerPerceptronManager.SetInputs(0, 1, 1);
					self.Log($"结果 {self.multilayerPerceptronManager.layers[self.multilayerPerceptronManager.layers.Count - 1].nodes[0].result}");
				}

				if (Input.GetKeyDown(KeyCode.V))
				{
					self.multilayerPerceptronManager.SetInputs(0, 0, 1);
					self.Log($"结果 {self.multilayerPerceptronManager.layers[self.multilayerPerceptronManager.layers.Count - 1].nodes[0].result}");
				}
			}
		}
		/// <summary>
		/// 训练
		/// </summary>
		public static void Exercise(this MultilayerPerceptronTest self)
		{
			self.multilayerPerceptronManager.SetInputs(0, 0, 0);
			self.multilayerPerceptronManager.SetOutputs(0);

			self.multilayerPerceptronManager.SetInputs(0, 1, 0);
			self.multilayerPerceptronManager.SetOutputs(1);

			self.multilayerPerceptronManager.SetInputs(1, 0, 0);
			self.multilayerPerceptronManager.SetOutputs(1);

			self.multilayerPerceptronManager.SetInputs(1, 1, 0);
			self.multilayerPerceptronManager.SetOutputs(0);


			self.multilayerPerceptronManager.SetInputs(0, 0, 1);
			self.multilayerPerceptronManager.SetOutputs(0);

			self.multilayerPerceptronManager.SetInputs(0, 1, 1);
			self.multilayerPerceptronManager.SetOutputs(1);


			self.multilayerPerceptronManager.SetInputs(1, 0, 1);
			self.multilayerPerceptronManager.SetOutputs(1);

			self.multilayerPerceptronManager.SetInputs(1, 1, 1);
			self.multilayerPerceptronManager.SetOutputs(0);
		}
	}

}
