
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/23 16:04

* 描述： Addressables 的异步扩展

*/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace WorldTree
{

    public static class AddressablesAsyncExtension
    {
        public async static AsyncTask<T> AddressablesLoadAssetAsync<T>(this Entity self, string key)
        {
            return (await self.GetAwaiter(Addressables.LoadAssetAsync<T>(key))).Result;
        }

        public async static AsyncTask<IList<T>> AddressablesLoadAssetsAsync<T>(this Entity self, string key, Action<T> CallBack = null)
        {
            return (await self.GetAwaiter(Addressables.LoadAssetsAsync<T>(key, CallBack))).Result;
        }

        public async static AsyncTask<GameObject> AddressablesInstantiateAsync(this Entity self, string key)
        {
            return (await self.GetAwaiter(Addressables.InstantiateAsync(key))).Result;
        }
    }
}
