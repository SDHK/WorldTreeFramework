/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System.Collections.Generic;
using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// Unity可视化绑定器世界
	/// </summary>
	public class UnityViewBuilderWorld : World
	{
	}

	/// <summary>
	/// Unity的世界树节点可视化生成器
	/// </summary>
	/// <remarks>负责Node映射显示的gameObjcet生成</remarks>
	public class UnityWorldTreeNodeViewBuilder : Node, IWorldTreeNodeViewBuilder
		, ChildOf<INode>
		, AsRule<Awake<INode, INode>>
	{
		/// <summary>
		/// 节点
		/// </summary>
		public INode node;
		/// <summary>
		/// 游戏对象
		/// </summary>
		public GameObject gameObject;
		/// <summary>
		/// 父分支对象
		/// </summary>
		public GameObject parentBranchObj;
		/// <summary>
		/// 分支对象字典
		/// </summary>
		public Dictionary<long, GameObject> branchObjDict = new Dictionary<long, GameObject>();
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
					UnityWorldTreeNodeView monoView = self.gameObject.AddComponent<UnityWorldTreeNodeView>();
					monoView.Node = node;
					monoView.View = self;
				}
				if (parent != null)
				{
					UnityWorldTreeNodeViewBuilder parentView = parent.ViewBuilder as UnityWorldTreeNodeViewBuilder;
					GameObject parentObj = parentView.gameObject;
					if (!parentView.branchObjDict.TryGetValue(node.BranchType, out self.parentBranchObj))
					{
						self.parentBranchObj = new GameObject(node.CodeToType(node.BranchType).Name);
						self.parentBranchObj.transform.SetParent(parentObj.transform);
						parentView.branchObjDict.Add(node.BranchType, self.parentBranchObj);
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
				self.branchObjDict.Clear();
				self.node = null;
				self.parentBranchObj = null;
				self.gameObject = null;
			}
		}
	}
}

