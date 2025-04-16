/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	///	球体测试
	/// </summary>
	public class SphereTest : Node
		, AsAwake
		, TypeNodeOf<long, SphereManagerTest>
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

	}


}
