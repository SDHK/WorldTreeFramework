﻿using UnityEngine;

namespace WorldTree
{
    public static partial class GameObjectEntityExtension
    {

        /// <summary>
        /// 添加游戏对象实体:通过类名加载预制体，实例化后添加组件。
        /// </summary>
        public static async AsyncTask<T> AddGameObjectEntity<T>(this Entity self)
        where T : Entity
        {
            GameObject prefab = await self.AddressablesLoadAssetAsync<GameObject>(typeof(T).Name);
            return self.AddChildren<GameObjectEntity>().Instantiate(prefab).AddComponent<T>();
        }
    }
}
