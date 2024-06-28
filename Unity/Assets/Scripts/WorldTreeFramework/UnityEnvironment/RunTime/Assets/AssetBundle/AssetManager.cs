/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/14 11:37

* 描述： 资源管理器

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 资源管理器
	/// </summary>
	public class AssetManager : Node
		, ComponentOf<WorldTreeRoot>
		, AsLoadAssetAsync
	{
		/// <summary>
		/// 资源字典
		/// </summary>
		public TreeDictionary<string, Object> assetDict;

		/// <summary>
		/// 异步加载资源
		/// </summary>
		public async TreeTask<T> LoadAssetAsync<T, N>()
			where T : class
			where N : class, INode
		{
			var key = typeof(N);
			if (!assetDict.TryGetValue(key.Name, out var asset))
			{
				asset = await this.LoadAssetAsync(key.Name);
				assetDict.Add(key.Name, asset);
			}
			else
			{
				await this.TreeTaskCompleted();
			}
			return asset as T;
		}
	}
}
