/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System;

//using UnityEngine.AddressableAssets;

namespace WorldTree
{
	/// <summary>
	/// Addressables 资源加载管理器
	/// </summary>
	public class AddressablesManager : Node, ComponentOf<World>
		, AsAwake
	{
		/// <summary>
		/// 资源字典
		/// </summary>
		public TreeDictionary<string, Object> assetDict;

		/// <summary>
		/// 根据节点类型步加载资源
		/// </summary>
		/// <typeparam name="T">资源类型</typeparam>
		/// <typeparam name="N">节点类型</typeparam>
		public async TreeTask<T> LoadAssetAsync<T, N>()
			where T : class
			where N : class, INode
		{
			var key = typeof(N);
			if (!assetDict.TryGetValue(key.Name, out var asset))
			{
				//asset = (await this.GetAwaiter(Addressables.LoadAssetAsync<T>(key.Name))).Result;
				assetDict.Add(key.Name, asset);
			}
			else
			{
				await this.TreeTaskCompleted();
			}
			return asset as T;
		}
	}

	//class AddressablesManagerAddRule : AddRule<AddressablesManager>
	//{
	//    protected override void OnEvent(AddressablesManager self)
	//    {
	//        self.AddChild(out self.assets);
	//    }
	//}
}