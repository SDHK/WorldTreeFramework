/****************************************

* 作者： 闪电黑客
* 日期： 2024/01/24 06:17:03

* 描述：世界树框架节点Unity显示

*/

using System.Collections.Generic;
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// 世界树节点Unity可视化节点
	/// </summary>
	public class TreeNodeUnityView : Node, IWorldTreeNodeView
		, ChildOf<WorldTreeRoot>
		, AsRule<IAwakeRule<INode, INode>>
	{
		public INode node;
		public GameObject gameObject;
		public Dictionary<long, GameObject> BranchObjs = new Dictionary<long, GameObject>();
	}

	public static class TreeNodeUnityViewRule
	{
		class AwakeRule : AwakeRule<TreeNodeUnityView, INode, INode>
		{
			protected override void OnEvent(TreeNodeUnityView self, INode node, INode parent)
			{
				self.node = node;
				self.gameObject ??= new GameObject(node.GetType().Name);
				if (self.gameObject != null)
				{
					WorldTreeNodeViewMonoComponent MonoView = self.gameObject.AddComponent<WorldTreeNodeViewMonoComponent>();
					MonoView.Node = node;
					MonoView.View = self;
				}
				if (parent != null)
				{
					TreeNodeUnityView parentView = parent.View as TreeNodeUnityView;
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
		class RemoveRule : RemoveRule<TreeNodeUnityView>
		{
			protected override void OnEvent(TreeNodeUnityView self)
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

