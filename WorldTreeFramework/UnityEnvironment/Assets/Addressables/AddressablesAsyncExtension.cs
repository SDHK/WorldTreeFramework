/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/23 16:04

* 描述： Addressables 的异步扩展

*/

namespace WorldTree
{
    /// <summary>
    /// Addressables 异步扩展
    /// </summary>
    public static class AddressablesAsyncExtension
    {
        /// <summary>
        /// Addressables加载资源
        /// </summary>
        public async static TreeTask<T> AddressablesLoadAssetAsync<T, N>(this INode self)
            where T : class
            where N : class,INode
        {
            return await self.Core.AddComponent<AddressablesManager>().LoadAssetAsync<T,N>();
        }
    }
}
