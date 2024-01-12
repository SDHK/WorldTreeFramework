#if UNITY_EDITOR

using UnityEngine;

namespace WorldTree
{
	public class TreeNodeMonoView : MonoBehaviour
	{
		public INode Node;

		public void Start()
		{
			gameObject.name = Node.GetType().Name;
		}
	}


	public class TreeNodeView : UnitPoolItem, ITreeNodeView
	{
		private INode node;
		public GameObject gameObject;

		public void Draw(INode node, INode parent = null)
		{
			this.node = node;
			gameObject ??= new GameObject(node.GetType().Name);
			if (gameObject != null) gameObject.AddComponent<TreeNodeMonoView>().Node = node;
			if (parent != null)
			{
				GameObject parentObj = (parent.View as TreeNodeView).gameObject;
				if (parentObj != null)
				{
					gameObject?.transform.SetParent(parentObj.transform);
				}
			}
			else
			{
				GameObject.DontDestroyOnLoad(gameObject);
			}
		}
		public override void OnRecycle()
		{
			this.node = null;
			if (gameObject != null) GameObject.Destroy(gameObject);
			base.OnRecycle();
		}
	}
}
#endif

