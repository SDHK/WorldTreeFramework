
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
                asset = await this.CallRuleAsync((ILoadAssetAsyncRule)null, key.Name, (Object)null);
                assets.Add(key.Name, asset);
            }
            else
            {
                await this.TreeTaskCompleted();
            }
            return asset as T;
        }
    }
}
