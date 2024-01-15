#if UNITY_EDITOR

using System.Collections.Generic;
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
	{
		public INode node;
		public GameObject gameObject;
		public Dictionary<long, GameObject> BranchObjs = new Dictionary<long, GameObject>();
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
					WorldTreeNodeUnityView parentView = parent.View as WorldTreeNodeUnityView;
					GameObject parentObj = parentView.gameObject;
					if (!parentView.BranchObjs.TryGetValue(node.BranchType, out GameObject parentBranchObj))
					{
						parentBranchObj = new GameObject(node.BranchType.CoreToType().Name);
						parentBranchObj.transform.SetParent(parentObj.transform);
						parentView.BranchObjs.Add(node.BranchType, parentBranchObj);
					}
					self.gameObject.transform.SetParent(parentBranchObj.transform);
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

				var parentBranchObj = self.gameObject.transform.parent;

				if (self.gameObject != null) GameObject.Destroy(self.gameObject);
				if (parentBranchObj != null && parentBranchObj.childCount == 0)
				{
					GameObject.Destroy(parentBranchObj.gameObject);
				}
				self.BranchObjs.Clear();
			}
		}


	}

}
#endif

