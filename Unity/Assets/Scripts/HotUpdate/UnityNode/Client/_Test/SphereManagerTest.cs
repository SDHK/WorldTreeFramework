/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// 球体管理器测试
	/// </summary>
	public class SphereManagerTest : Node
		, ComponentOf<InitialDomain>
		, AsTypeNodeBranch<long>
		, AsAwake
	{
		/// <summary>
		/// 重力
		/// </summary>
		public int G = 10;
		/// <summary>
		/// 球体半径
		/// </summary>
		public int BottomY = 0;
		/// <summary>
		/// 顶部Y坐标
		/// </summary>
		public int TopY = 10;
		/// <summary>
		/// 生成数量
		/// </summary>
		public int SpawnCount = 1000;
		/// <summary>
		/// 球体
		/// </summary>
		public GameObject balls;
	}
}