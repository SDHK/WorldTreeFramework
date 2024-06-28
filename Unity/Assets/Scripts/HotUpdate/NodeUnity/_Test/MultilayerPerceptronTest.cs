using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// 多层感知机测试
	/// </summary>
	public class MultilayerPerceptronTest : Node, ComponentOf<InitialDomain>
		, AsComponentBranch
		, AsAwake
	{
		/// <summary>
		/// 多层感知机管理器
		/// </summary>
		public MultilayerPerceptronManager multilayerPerceptronManager;
	}
}
