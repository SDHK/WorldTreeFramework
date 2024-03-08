using System;
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

	
}
