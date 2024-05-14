using UnityEngine;

namespace WorldTree
{
	public class MultilayerPerceptronTest : Node, ComponentOf<InitialDomain>
		, AsAwake
	{
		public MultilayerPerceptronManager multilayerPerceptronManager;
	}
}
