/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// GameObject节点助手
	/// </summary>
	public static class GameObjectNodeHelper
	{

		/// <summary>
		/// 添加游戏对象实体:通过类名加载预制体，实例化后添加组件。
		/// </summary>
		public static async TreeTask<T> AddGameObjectNode<N, T>( N self)
			where N : class, AsChildBranch
			where T : class, INode, ComponentOf<INode>, AsRule<Awake>
		{
			GameObject prefab = await self.Root.AddComponent(out AddressablesManager _).LoadAssetAsync<GameObject, T>();
			return self.AddChild(out GameObjectNode _).Instantiate(prefab).AddComponent(out T _);
		}

		/// <summary>
		/// 添加游戏对象实体:通过类名加载预制体，实例化后添加组件。
		/// </summary>
		public static async TreeTask<T> AddGameObjectNode<N, T>( N self, Transform parent)
			where N : class, AsChildBranch
			where T : class, INode, ComponentOf<INode>, AsRule<Awake>
		{
			GameObject prefab = await self.Root.AddComponent(out AddressablesManager _).LoadAssetAsync<GameObject, T>();
			return self.AddChild(out GameObjectNode _).Instantiate(prefab, parent).AddComponent(out T _);
		}

		/// <summary>
		/// 添加游戏对象实体:通过类名加载预制体，实例化后添加组件。
		/// </summary>
		public static async TreeTask<T> AddGameObjectNode<N, T>( N self, GameObjectNode parent)
			where N : class, AsChildBranch
			where T : class, INode, ComponentOf<INode>, AsRule<Awake>
		{
			GameObject prefab = await self.Root.AddComponent(out AddressablesManager _).LoadAssetAsync<GameObject, T>();
			return self.AddChild(out GameObjectNode _).Instantiate(prefab, parent).AddComponent(out T _);
		}
	}
}
