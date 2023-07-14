
/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/14 11:37

* 描述： 资源管理器

*/

using System;

namespace WorldTree
{
    public class AssetManager : Node, ComponentOf<WorldTreeRoot>
        , AsRule<ILoadAssetAsyncRule>
    {
        public TreeDictionary<string, Object> assets;

        public async TreeTask<T> LoadAssetAsync<T, N>()
            where T : class
            where N : class, INode
        {
            var key = typeof(N);
            if (!assets.TryGetValue(key.Name, out var asset))
            {
                asset = await this.CallRuleAsync(default(ILoadAssetAsyncRule), key.Name, default(Object));
                assets.Add(key.Name, asset);
            }
            else
            {
                await this.TreeTaskCompleted();
            }
            return asset as T;
        }
    }

    /// <summary>
    /// 异步加载资源法则接口
    /// </summary>
    public interface ILoadAssetAsyncRule : ICallRuleAsync<string, Object> { }
    /// <summary>
    /// 异步加载资源法则
    /// </summary>
    public abstract class LoadAssetAsyncRule<N> : CallRuleAsyncBase<N, ILoadAssetAsyncRule, string, Object> where N : class, INode, AsRule<ILoadAssetAsyncRule> { }

}
