using UnityEngine;

namespace WorldTree
{
	public class MultilayerPerceptronTest : Node, ComponentOf<InitialDomain>
		, AsRule<IAwakeRule>
	{
		public MultilayerPerceptronManager multilayerPerceptronManager;
	}
}
