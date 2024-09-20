using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// 球体管理器测试
	/// </summary>
	public class SphereManagerTest : Node
		, ComponentOf<InitialDomain>
		, AsNumberNodeBranch
		, AsAwake
	{
		/// <summary>
		/// 重力
		/// </summary>
		public int G = 10;
		/// <summary>
		/// 球体半径
		/// </summary>
		public int bottomY = 0;
		/// <summary>
		/// 顶部Y坐标
		/// </summary>
		public int topY = 10;
		/// <summary>
		/// 生成数量
		/// </summary>
		public int spawnCount = 1000;
		/// <summary>
		/// 球体
		/// </summary>
		public GameObject balls;
	}
}