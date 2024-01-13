#if UNITY_EDITOR

using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace WorldTree
{
	public class WorldTreeNodeMonoView : MonoBehaviour
	{
		public INode Node;
		public IWorldTreeNodeView View;

		public void Start()
		{
			gameObject.name = Node.GetType().Name;
		}
	}


	public class WorldTreeNodeUnityView : Node, IWorldTreeNodeView
		, ChildOf<WorldTreeRoot>
		, AsRule<IAwakeRule<INode, INode>>
		, AsRule<IWorldTreeNodeFieldInfoViewRule>
	{
		public INode node;
		public GameObject gameObject;
	}

	public static class TreeNodeViewRule
	{
		class AwakeRule : AwakeRule<WorldTreeNodeUnityView, INode, INode>
		{
			protected override void OnEvent(WorldTreeNodeUnityView self, INode node, INode parent)
			{
				self.node = node;
				self.gameObject ??= new GameObject(node.GetType().Name);
				if (self.gameObject != null)
				{
					WorldTreeNodeMonoView MonoView = self.gameObject.AddComponent<WorldTreeNodeMonoView>();
					MonoView.Node = node;
					MonoView.View = self;
				}
				if (parent != null)
				{
					GameObject parentObj = (parent.View as WorldTreeNodeUnityView).gameObject;
					if (parentObj != null)
					{
						self.gameObject?.transform.SetParent(parentObj.transform);
					}
				}
				else
				{
					GameObject.DontDestroyOnLoad(self.gameObject);
				}
			}
		}
		class RemoveRule : RemoveRule<WorldTreeNodeUnityView>
		{
			protected override void OnEvent(WorldTreeNodeUnityView self)
			{
				self.node = null;
				if (self.gameObject != null) GameObject.Destroy(self.gameObject);
			}
		}


	}
	
}
#endif

