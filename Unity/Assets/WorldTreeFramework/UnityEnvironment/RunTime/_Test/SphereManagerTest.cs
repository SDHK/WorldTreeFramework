using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WorldTree
{
	public class SphereManagerTest : Node
		, ComponentOf<InitialDomain>
		, AsRule<IAwakeRule>
	{
		public int G = 10;
		public int bottomY = 0;
		public int topY = 10;
		public int spawnCount = 7000;
		public GameObject balls;
	}

	public static class SphereManagerTestRule
	{

		class UpdateRule : UpdateRule<SphereManagerTest>
		{
			protected override void OnEvent(SphereManagerTest self)
			{
				if (Input.GetKeyDown(KeyCode.Q))
				{
					self.Run();
				}
			}
		}


		private static void Run(this SphereManagerTest self)
		{
			self.balls = GameObject.Find("Sphere");
			GameObject rootGo = new GameObject("SphereManagerTest");
			rootGo.transform.position = Vector3.zero;

			for (int i = 0; i < self.spawnCount; ++i)
			{
				var go = UnityEngine.Object.Instantiate(self.balls);
				go.name = "Drop_" + i;

				self.AddIdNode(i, out SphereTest dropComponent);
				dropComponent.manager = self;
				dropComponent.gameObject = go;
				//dropComponent.delay = 0.02f * i;
				dropComponent.mass = UnityEngine.Random.Range(0.5f, 3f);

				Vector3 pos = UnityEngine.Random.insideUnitSphere * 40;
				go.transform.parent = rootGo.transform;
				pos.y = self.topY;
				go.transform.position = pos;
			}
		}
	}
}
