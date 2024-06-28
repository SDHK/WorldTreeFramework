using YooAsset;

namespace WorldTree
{
	/// <summary>
	///	YooAsset测试
	/// </summary>
	public class YooAssetTest : Node, ComponentOf<InitialDomain>
	, AsAwake
	{
		/// <summary>
		/// 资源包
		/// </summary>
		public ResourcePackage package;
	}
}
