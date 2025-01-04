/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System;
using System.Collections.Generic;

namespace WorldTree
{

	//class InitialDomain1 : AddNodeRule<InitialDomain>
	//{
	//    protected override void OnEvent(InitialDomain self)
	//    {
	//        self.Core.AddComponent<WindowManager>();
	//    }
	//}

	/// <summary>
	/// UI 栈窗口管理器
	/// </summary>
	public class WindowManager : Node
		, AsChildBranch
		, AsComponentBranch
		, AsAwake
	{
		/// <summary>
		/// 全部窗口
		/// </summary>
		public UnitDictionary<long, INode> windowDict = new UnitDictionary<long, INode>();
		/// <summary>
		/// 栈窗口
		/// </summary>
		public Stack<INode> windowStack = new Stack<INode>();

		/// <summary>
		/// 根节点
		/// </summary>
		public GameObjectNode gameObject;

		/// <summary>
		/// 打开窗口入栈
		/// </summary>
		public async TreeTask<T> Show<T>()
			where T : class, INode, ComponentOf<INode>, AsRule<Awake>

		{
			if (windowDict.TryGetValue(Core.TypeToCode<T>(), out INode node))
			{
				if (windowStack.TryPeek(out INode outNode))
				{
					NodeRuleHelper.TrySendRule(outNode, default(WindowLostFocus));
				}
				while (windowStack.Count != 0 ? windowStack.Peek().Id != node.Id : false)
				{
					outNode = windowStack.Pop();
					windowDict.Remove(outNode.Type);
					if (outNode.TryGetParent(out GameObjectNode gameObjectNode)) gameObjectNode.Dispose();
				}
				NodeRuleHelper.TrySendRule(node, default(WindowFocus));

				await this.TreeTaskCompleted();
			}
			else
			{
				node = await GameObjectNodeHelper.AddGameObjectNode<WindowManager, T>(this, gameObject);

				windowDict.Add(node.Type, node);

				if (windowStack.TryPeek(out INode topNode))
				{
					NodeRuleHelper.TrySendRule(topNode, default(WindowLostFocus));
				}
				windowStack.Push(node);

				NodeRuleHelper.TrySendRule(node, default(WindowFocus));
			}
			return node as T;
		}


		/// <summary>
		/// 关闭栈窗口
		/// </summary>
		public void Dispose<T>()
		   where T : class, INode
		{
			if (windowDict.TryGetValue(Core.TypeToCode<T>(), out INode targetNode))
			{
				if (windowStack.TryPeek(out INode topNode))
				{
					NodeRuleHelper.TrySendRule(topNode, default(WindowLostFocus));
				}
				while (windowStack.TryPop(out topNode))
				{
					if (topNode.Id == targetNode.Id)
					{
						windowDict.Remove(topNode.Type);
						if (topNode.TryGetParent(out GameObjectNode node)) node.Dispose();
						break;
					}
				}
				if (windowStack.TryPeek(out topNode))
				{
					NodeRuleHelper.TrySendRule(topNode, default(WindowFocus));
				}
			}
		}

		/// <summary>
		/// 关闭栈顶
		/// </summary>
		public void DisposeTop()
		{
			if (windowStack.TryPop(out INode outNode))
			{
				windowDict.Remove(outNode.Type);
				NodeRuleHelper.TrySendRule(outNode, default(WindowLostFocus));
				if (windowStack.TryPeek(out INode topNode))
				{
					NodeRuleHelper.TrySendRule(topNode, default(WindowFocus));
				}
				if (outNode.TryGetParent(out GameObjectNode gameObjectNode)) gameObjectNode.Dispose();
			}
		}
		/// <summary>
		/// 关闭全部
		/// </summary>
		public void DisposeAll()
		{
			if (windowStack.TryPeek(out INode topNode))
			{
				NodeRuleHelper.TrySendRule(topNode, default(WindowFocus));
			}
			while (windowStack.TryPop(out topNode))
			{
				if (topNode.TryGetParent(out GameObjectNode gameObjectNode)) gameObjectNode.Dispose();
			}
		}


		/// <summary>
		/// 关闭动画栈窗口
		/// </summary>
		public void Close<T>()
		   where T : class, INode
		{

		}

		/// <summary>
		/// 关闭动画栈顶窗口
		/// </summary>
		public void CloseTop()
		{
		}
	}


	class WindowManagerAddRule : AddRule<WindowManager>
	{
		protected override void Execute(WindowManager self)
		{
			self.Log("WindowManager启动!!!");
			self.gameObject = self.AddComponent<WindowManager, GameObjectNode>(out GameObjectNode _).Instantiate<WindowManager>();
		}
	}

	class WindowManagerUpdateRule : UpdateTimeRule<WindowManager>
	{
		protected override void Execute(WindowManager self, TimeSpan deltaTime)
		{
			if (self.windowStack.Count != 0)
			{
				if (self.windowStack.TryPeek(out INode node))
				{
					NodeRuleHelper.TrySendRule(node, default(WindowFocusUpdate), deltaTime);
				}
			}
		}
	}

	//class WindowManagerNodeAddRule : ListenerAddRule<WindowManager>
	//{
	//    protected override void OnEvent(WindowManager self, INode node)
	//    {

	//    }
	//}
	//class WindowManagerNodeRemoveRule : ListenerRemoveRule<WindowManager>
	//{
	//    protected override void OnEvent(WindowManager self, INode node)
	//    {

	//    }
	//}


}