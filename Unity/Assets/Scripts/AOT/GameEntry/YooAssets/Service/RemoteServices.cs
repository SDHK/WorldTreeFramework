/****************************************

* 作者： 闪电黑客
* 日期： 2024/3/19 20:30

* 描述：

*/

using YooAsset;

namespace WorldTree.AOT
{
	/// <summary>
	/// 远端资源地址查询服务类
	/// </summary>
	public class RemoteServices : IRemoteServices
	{
		private readonly string _defaultHostServer;
		private readonly string _fallbackHostServer;

		public RemoteServices(string defaultHostServer, string fallbackHostServer)
		{
			_defaultHostServer = defaultHostServer;
			_fallbackHostServer = fallbackHostServer;
		}

		public string GetRemoteFallbackURL(string fileName)
		{
			return $"{_fallbackHostServer}/{fileName}";
		}

		public string GetRemoteMainURL(string fileName)
		{
			return $"{_defaultHostServer}/{fileName}";
		}
	}
}