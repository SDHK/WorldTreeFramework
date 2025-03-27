/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// 游戏物体实体
	/// </summary>
	public class GameObjectNode : Node, ComponentOf<INode>, ChildOf<INode>
		, AsComponentBranch
		, AsAwake
	{
		/// <summary>
		/// 对象池
		/// </summary>
		public GameObjectPool pool;

		/// <summary>
		/// 游戏物体
		/// </summary>
		public GameObject gameObject;
		/// <summary>
		/// 物体变换
		/// </summary>
		public Transform transform;

		public override string ToString()
		{
			return gameObject ? "GameObjectNode : " + gameObject.name : "GameObjectNode : ";
		}

		/// <summary>
		/// 回收或删除游戏物体
		/// </summary>
		public void DestroyGameObject()
		{
			if (gameObject != null)
			{
				if (pool != null)
				{
					pool.Recycle(gameObject);
				}
				else
				{
					GameObject.Destroy(gameObject);
				}
			}
		}

		/// <summary>
		/// 用类名实例化一个空物体
		/// </summary>
		public GameObjectNode Instantiate(string name)
		{
			DestroyGameObject();
			gameObject = new GameObject(name);
			transform = gameObject.transform;
			transform.Default();
			return this;
		}

		/// <summary>
		/// 用类名实例化一个空物体
		/// </summary>
		public GameObjectNode Instantiate(string name, Transform parent)
		{
			Instantiate(name);
			transform.SetParent(parent);
			return this;
		}
		/// <summary>
		/// 用类名实例化一个空物体
		/// </summary>
		public GameObjectNode Instantiate(string name, GameObjectNode parent)
		{
			Instantiate(name);
			transform.SetParent(parent.transform);
			return this;
		}


		/// <summary>
		/// 用类名实例化一个空物体
		/// </summary>
		public GameObjectNode Instantiate<T>() where T : class => Instantiate(typeof(T).Name);

		/// <summary>
		/// 用类名实例化一个空物体
		/// </summary>
		public GameObjectNode Instantiate<T>(Transform parent) where T : class => Instantiate(typeof(T).Name, parent);

		/// <summary>
		/// 用类名实例化一个空物体
		/// </summary>
		public GameObjectNode Instantiate<T>(GameObjectNode parent) where T : class => Instantiate(typeof(T).Name, parent);


		/// <summary>
		/// 尝试实例化预制体
		/// </summary>
		public bool TryInstantiate(GameObject prefab, out GameObject obj)
		{
			if (prefab)
			{
				DestroyGameObject();
				pool = this.World.AddComponent(out GameObjectPoolManager _).GetPool(prefab);
				gameObject = pool.Get();
				transform = gameObject.transform;

				obj = gameObject;
				return true;
			}
			else
			{
				obj = null;
				return false;
			}
		}



		/// <summary>
		/// 实例化
		/// </summary>
		public GameObjectNode Instantiate(GameObject prefab)
		{
			if (prefab)
			{
				DestroyGameObject();
				pool = this.World.AddComponent(out GameObjectPoolManager _).GetPool(prefab);
				gameObject = pool.Get();
				transform = gameObject.transform;
			}
			return this;
		}

		/// <summary>
		/// 实例化
		/// </summary>
		public GameObjectNode Instantiate(GameObject prefab, Transform parent)
		{
			if (TryInstantiate(prefab, out GameObject gameObject))
			{
				gameObject.transform.SetParent(parent);
			}
			return this;
		}

		/// <summary>
		/// 实例化
		/// </summary>
		public GameObjectNode Instantiate(GameObject prefab, GameObjectNode parent)
		{
			if (TryInstantiate(prefab, out GameObject gameObject))
			{
				gameObject.transform.SetParent(parent.transform);
			}
			return this;
		}
	}

	class GameObjectEntityRemoveRule : RemoveRule<GameObjectNode>
	{
		protected override void Execute(GameObjectNode self)
		{
			self.DestroyGameObject();
			self.gameObject = null;
			self.transform = null;
			self.pool = null;
		}
	}
}
