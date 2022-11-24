using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WorldTree
{

    // 通过构造系统实现
    class GameObjectComponentAwakeSystem : AwakeSystem<GameObjectComponent>
    {
        public override async void OnAwake(GameObjectComponent self)
        {
            var R = await self.AddressablesLoadAssetAsync<GameObject>(self.Parent.Type.Name);
            self.Instantiate(R.Result);
        }
    }


    //通过扩展方法实现
    public static class GameObjectComponentExtension
    {
        public static async AsyncTask<GameObjectComponent> InstantiateAsync(this Entity self)
        {
            var R = await self.AddressablesLoadAssetAsync<GameObject>(self.Type.Name);
            return self.AddComponent<GameObjectComponent>().Instantiate(R.Result);
        }
    }
}
