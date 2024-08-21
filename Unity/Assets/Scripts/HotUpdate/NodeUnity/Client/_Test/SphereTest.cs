using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	///	球体测试
	/// </summary>
	public class SphereTest : Node
		, AsAwake
	{
		/// <summary>
		/// 球体
		/// </summary>
		public GameObject gameObject;
		/// <summary>
		/// 球体管理器
		/// </summary>
		public SphereManagerTest manager;
		/// <summary>
		/// 球体数据
		/// </summary>
		public float mass;
		/// <summary>
		/// 球体数据
		/// </summary>
		public float delay;
		/// <summary>
		/// 球体数据
		/// </summary>
		public float velocity;

	}


}
