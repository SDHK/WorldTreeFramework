
/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/10 10:33

* 描述： Addressables 资源加载管理器

*/

using System;
//using UnityEngine.AddressableAssets;

namespace WorldTree
{
    /// <summary>
    /// Addressables 资源加载管理器
    /// </summary>
    public class AddressablesManager : Node, ComponentOf<WorldTreeRoot>
        ,AsRule<IAwakeRule>
        ,AsRule<IAddRule>
    {
        public TreeDictionary<string, Object> assets;

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
            if (!assets.TryGetValue(key.Name, out var asset))
            {
                //asset = (await this.GetAwaiter(Addressables.LoadAssetAsync<T>(key.Name))).Result;
                assets.Add(key.Name, asset);
            }
            else
            {
                await this.TreeTaskCompleted();
            }
            return asset as T;
        }

    }


    class AddressablesManagerAddRule : AddRule<AddressablesManager>
    {
        public override void OnEvent(AddressablesManager self)
        {
           self.AddChild(out self.assets);
        }
    }

}
