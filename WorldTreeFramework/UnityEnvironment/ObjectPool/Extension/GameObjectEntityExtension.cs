using UnityEngine;

namespace WorldTree
{
    public static class GameObjectEntityExtension
    {

        /// <summary>
        /// 添加游戏对象实体:通过类名加载预制体，实例化后添加组件。
        /// </summary>
        public static async TreeTask<T> AddGameObjectNode<T>(this Node self)
        where T : Node
        {
            GameObject prefab = await self.AddressablesLoadAssetAsync<GameObject, T>();
            return self.AddChildren<GameObjectNode>().Instantiate(prefab).AddComponent<T>();
        }

        /// <summary>
        /// 添加游戏对象实体:通过类名加载预制体，实例化后添加组件。
        /// </summary>
        public static async TreeTask<T> AddGameObjectNode<T>(this Node self, Transform parent)
        where T : Node
        {
            GameObject prefab = await self.AddressablesLoadAssetAsync<GameObject, T>();
            return self.AddChildren<GameObjectNode>().Instantiate(prefab, parent).AddComponent<T>();
        }

        /// <summary>
        /// 添加游戏对象实体:通过类名加载预制体，实例化后添加组件。
        /// </summary>
        public static async TreeTask<T> AddGameObjectNode<T>(this Node self, GameObjectNode parent)
        where T : Node
        {
            GameObject prefab = await self.AddressablesLoadAssetAsync<GameObject, T>();
            return self.AddChildren<GameObjectNode>().Instantiate(prefab, parent).AddComponent<T>();
        }
    }
}
