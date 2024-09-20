using UnityEngine;

namespace WorldTree
{
	public static class SphereManagerTestRule
	{
		private class UpdateRule : UpdateRule<SphereManagerTest>
		{
			protected override void Execute(SphereManagerTest self)
			{
				if (Input.GetKeyDown(KeyCode.Q))
				{
					self.Log($"SphereManagerTestRule！！!");
					self.Run();
				}
			}
		}

		/// <summary>
		/// 运行
		/// </summary>
		private static void Run(this SphereManagerTest self)
		{
			self.balls = GameObject.Find("Sphere");
			GameObject rootGo = new GameObject("SphereManagerTest");
			rootGo.transform.position = Vector3.zero;

			for (int i = 0; i < self.spawnCount; ++i)
			{
				var go = UnityEngine.Object.Instantiate(self.balls);
				go.name = "Drop_" + i;

				self.AddNumberNode(i, out SphereTest dropComponent);
				dropComponent.manager = self;
				dropComponent.gameObject = go;

				dropComponent.delay = 0.02f * i;
				dropComponent.mass = UnityEngine.Random.Range(0.5f, 3f);

				Vector3 pos = UnityEngine.Random.insideUnitSphere * 40;
				go.transform.parent = rootGo.transform;
				pos.y = self.topY;
				go.transform.position = pos;
			}
		}
	}
}