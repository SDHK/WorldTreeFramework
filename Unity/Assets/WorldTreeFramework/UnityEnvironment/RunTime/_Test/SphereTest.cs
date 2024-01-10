﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldTree
{
	public class SphereTest : Node
		, IdNodeOf<SphereManagerTest>
		, AsRule<IAwakeRule>
	{
		public GameObject gameObject;
		public SphereManagerTest manager;
		public float mass;
		public float delay;
		public float velocity;

	}

	public static class SphereTestRule
	{

		class UpdateTimeRule : UpdateTimeRule<SphereTest>
		{
			protected override void OnEvent(SphereTest self, TimeSpan deltaTime)
			{
				if (self.delay > 0)
				{
					self.delay -= (float)deltaTime.TotalSeconds;
				}
				else
				{
					Vector3 pos = self.gameObject.transform.position;
					float v = self.velocity + self.manager.G * self.mass * (float)deltaTime.TotalSeconds;
					pos.y += v;
					if (pos.y < self.manager.bottomY)
					{
						pos.y = self.manager.topY;
						self.velocity = 0f;
						self.delay = UnityEngine.Random.Range(0, 3f);
					}
					self.gameObject.transform.position = pos;
				}
			}
		}

	}
}
