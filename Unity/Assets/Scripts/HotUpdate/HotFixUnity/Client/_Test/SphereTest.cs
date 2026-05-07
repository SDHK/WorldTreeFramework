/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System;
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	///	球体测试
	/// </summary>
	public partial class SphereTest : Node
		, AsRule<Awake>
		, GenericOf<long, SphereManagerTest>
	{
		/// <summary>
		/// 球体
		/// </summary>
		public GameObject GameObject;
		/// <summary>
		/// 球体管理器
		/// </summary>
		public SphereManagerTest Manager;
		/// <summary>
		/// 球体数据
		/// </summary>
		public float Mass;
		/// <summary>
		/// 球体数据
		/// </summary>
		public float Delay;
		/// <summary>
		/// 球体数据
		/// </summary>
		public float Velocity;

		[NodeRule(nameof(UpdateTimeRule<SphereTest>))]
		private static void OnUpdateTimeRule(SphereTest self, TimeSpan deltaTime)
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
