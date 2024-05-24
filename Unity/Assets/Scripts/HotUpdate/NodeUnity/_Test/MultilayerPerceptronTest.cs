using UnityEngine;

namespace WorldTree
{
	public class MultilayerPerceptronTest : Node, ComponentOf<InitialDomain>
		, AsComponentBranch
		, AsAwake
	{
		public MultilayerPerceptronManager multilayerPerceptronManager;
	}
}
