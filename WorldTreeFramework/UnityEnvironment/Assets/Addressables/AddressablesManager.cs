
/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/10 10:33

* 描述： Addressables 资源加载管理器

*/

using System;
using UnityEngine.AddressableAssets;

namespace WorldTree
{
    /// <summary>
    /// Addressables 资源加载管理器
    /// </summary>
    public class AddressablesManager : Entity
    {
        public EntityDictionary<string, Object> assets;

        /// <summary>
        /// 根据实体类型步加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <typeparam name="E">实体类型</typeparam>
        public async AsyncTask<T> LoadAssetAsync<T,E>()
            where T : class
            where E : Entity
        {
            var key = typeof(E);
            if (!assets.Value.TryGetValue(key.Name, out var asset))
            {
                asset = (await this.GetAwaiter(Addressables.LoadAssetAsync<T>(key.Name))).Result;
                assets.Value.Add(key.Name, asset);
            }
            else
            {
                await this.AsyncTaskCompleted();
            }
            return asset as T;
        }

    }


    class AddressablesManagerAddSystem : AddSystem<AddressablesManager>
    {
        public override void OnAdd(AddressablesManager self)
        {
            self.assets = self.AddChildren<EntityDictionary<string, Object>>();
        }
    }

}
