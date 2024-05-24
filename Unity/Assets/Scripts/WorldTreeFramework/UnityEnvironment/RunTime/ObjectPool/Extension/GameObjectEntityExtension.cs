using UnityEngine;

namespace WorldTree
{
	public static class GameObjectEntityExtension
	{

		/// <summary>
		/// 添加游戏对象实体:通过类名加载预制体，实例化后添加组件。
		/// </summary>
		public static async TreeTask<T> AddGameObjectNode<N, T>(this N self)
			where N : class, AsChildBranch
			where T : class, INode, ComponentOf<INode>, AsRule<Awake>
		{
			GameObject prefab = await self.AddressablesLoadAssetAsync<GameObject, T>();
			return self.AddChild(out GameObjectNode _).Instantiate(prefab).AddComponent(out T _);
		}

		/// <summary>
		/// 添加游戏对象实体:通过类名加载预制体，实例化后添加组件。
		/// </summary>
		public static async TreeTask<T> AddGameObjectNode<N, T>(this N self, Transform parent)
			where N : class, AsChildBranch
			where T : class, INode, ComponentOf<INode>, AsRule<Awake>
		{
			GameObject prefab = await self.AddressablesLoadAssetAsync<GameObject, T>();
			return self.AddChild(out GameObjectNode _).Instantiate(prefab, parent).AddComponent(out T _);
		}

		/// <summary>
		/// 添加游戏对象实体:通过类名加载预制体，实例化后添加组件。
		/// </summary>
		public static async TreeTask<T> AddGameObjectNode<N, T>(this N self, GameObjectNode parent)
			where N : class, AsChildBranch
			where T : class, INode, ComponentOf<INode>, AsRule<Awake>
		{
			GameObject prefab = await self.AddressablesLoadAssetAsync<GameObject, T>();
			return self.AddChild(out GameObjectNode _).Instantiate(prefab, parent).AddComponent(out T _);
		}
	}
}
