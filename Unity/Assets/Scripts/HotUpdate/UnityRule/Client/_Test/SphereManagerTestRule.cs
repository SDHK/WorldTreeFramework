/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
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

			for (int i = 0; i < self.SpawnCount; ++i)
			{
				var go = UnityEngine.Object.Instantiate(self.balls);
				go.name = "Drop_" + i;

				self.AddNumberNode(i, out SphereTest dropComponent);
				dropComponent.Manager = self;
				dropComponent.GameObject = go;

				dropComponent.Delay = 0.02f * i;
				dropComponent.Mass = UnityEngine.Random.Range(0.5f, 3f);

				Vector3 pos = UnityEngine.Random.insideUnitSphere * 40;
				go.transform.parent = rootGo.transform;
				pos.y = self.TopY;
				go.transform.position = pos;
			}
		}
	}
}