using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldTree
{
	public class SphereTest : Node
		, IdNodeOf<SphereManagerTest>
		, AsAwake
	{
		public GameObject gameObject;
		public SphereManagerTest manager;
		public float mass;
		public float delay;
		public float velocity;

	}

	
}
