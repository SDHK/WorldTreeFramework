/****************************************

* 作者： 闪电黑客
* 日期： 2024/01/24 06:17:03

* 描述：Unity的世界树节点可视化生成器

*/

using System.Collections.Generic;
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// Unity的世界树节点可视化生成器
	/// </summary>
	/// <remarks>负责Node映射显示的gameObjcet生成</remarks>
	public class UnityWorldTreeNodeViewBuilder : Node, IWorldTreeNodeViewBuilder
		, ChildOf<WorldTreeRoot>
		, AsAwake<INode, INode>
	{
		public INode node;
		public GameObject gameObject;
		public GameObject parentBranchObj;
		public Dictionary<long, GameObject> BranchObjs = new Dictionary<long, GameObject>();
	}

	public static class TreeNodeUnityViewRule
	{
		class AwakeRule : AwakeRule<UnityWorldTreeNodeViewBuilder, INode, INode>
		{
			protected override void Execute(UnityWorldTreeNodeViewBuilder self, INode node, INode parent)
			{
				self.node = node;

				self.gameObject ??= new GameObject(node.GetType().Name);
				if (self.gameObject != null)
				{
					UnityWorldTreeNodeView MonoView = self.gameObject.AddComponent<UnityWorldTreeNodeView>();
					MonoView.Node = node;
					MonoView.View = self;
				}
				if (parent != null)
				{
					UnityWorldTreeNodeViewBuilder parentView = parent.View as UnityWorldTreeNodeViewBuilder;
					GameObject parentObj = parentView.gameObject;
					if (!parentView.BranchObjs.TryGetValue(node.BranchType, out self.parentBranchObj))
					{
						self.parentBranchObj = new GameObject(node.BranchType.CoreToType().Name);
						self.parentBranchObj.transform.SetParent(parentObj.transform);
						parentView.BranchObjs.Add(node.BranchType, self.parentBranchObj);
					}

					if (self.gameObject != null)
					{
						self.gameObject.transform.SetParent(self.parentBranchObj.transform);
					}
				}
				else
				{
					GameObject.DontDestroyOnLoad(self.gameObject);
				}
			}
		}
		class RemoveRule : RemoveRule<UnityWorldTreeNodeViewBuilder>
		{
			protected override void Execute(UnityWorldTreeNodeViewBuilder self)
			{
				if (self.gameObject != null) GameObject.Destroy(self.gameObject);
				if (self.parentBranchObj != null && self.parentBranchObj.transform.childCount == 0)
				{
					GameObject.Destroy(self.parentBranchObj.gameObject);
				}
				self.BranchObjs.Clear();
				self.node = null;
				self.parentBranchObj = null;
				self.gameObject = null;
			}
		}
	}
}

