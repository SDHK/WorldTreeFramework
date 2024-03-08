using YooAsset;

namespace WorldTree
{
	public class YooAssetTest : Node, ComponentOf<InitialDomain>
	, AsRule<IAwakeRule>
	{
		public ResourcePackage package;
	}
}
