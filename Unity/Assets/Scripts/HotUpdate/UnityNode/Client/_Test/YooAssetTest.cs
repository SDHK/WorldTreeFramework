/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
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
