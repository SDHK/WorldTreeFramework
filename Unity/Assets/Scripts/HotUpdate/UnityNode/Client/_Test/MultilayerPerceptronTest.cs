/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// 多层感知机测试
	/// </summary>
	public class MultilayerPerceptronTest : Node, ComponentOf<InitialDomain>
		, AsComponentBranch
		, AsRule<Awake>
	{
		/// <summary>
		/// 多层感知机管理器
		/// </summary>
		public MultilayerPerceptronManager multilayerPerceptronManager;
	}
}
