using System;
using UnityEngine;

namespace WorldTree
{
	public static class SphereTestRule
	{
		class UpdateTimeRule : UpdateTimeRule<SphereTest>
		{
			protected override void Execute(SphereTest self, TimeSpan deltaTime)
			{
				if (self.Delay > 0)
				{
					self.Delay -= (float)deltaTime.TotalSeconds;
				}
				else
				{
					Vector3 pos = self.GameObject.transform.position;
					float v = self.Velocity + self.Manager.G * self.Mass * (float)deltaTime.TotalSeconds;
					pos.y += v;
					if (pos.y < self.Manager.BottomY)
					{
						pos.y = self.Manager.TopY;
						self.Velocity = 0f;
						self.Delay = UnityEngine.Random.Range(0, 3f);
					}
					self.GameObject.transform.position = pos;
				}
			}
		}
	}
}
