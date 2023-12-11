/****************************************

* 作者： 闪电黑客
* 日期： 2023/12/11 11:52:09

* 描述： 法则执行器基类
* 
* 这是一个可以在遍历时，增加和移除节点的队列遍历器。
* 在遍历时，如果增加了节点，那么会在下一次遍历时执行。
* 
* 如果移除了节点，那么就会被抵消跳过，不会执行。
* 如果节点被意外回收了，那么也会被跳过，不会执行。

*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace WorldTree
{
	/// <summary>
	/// 法则执行器基类
	/// </summary>
	public class RuleActuatorBase : Node, IRuleActuator, IRuleActuatorEnumerable
	{
		/// <summary>
		/// 节点法则队列
		/// </summary>
		/// <remarks>高速遍历执行的队列</remarks>
		public TreeQueue<ValueTuple<NodeRef<INode>, RuleList>> nodeRuleQueue;

		/// <summary>
		/// 节点Id字典
		/// </summary>
		/// <remarks>确保同一时刻不允许有两个相同的节点</remarks>
		public TreeHashSet<long> nodeIdHash;

		/// <summary>
		/// 节点id被移除的次数
		/// </summary>
		/// <remarks>队列是无法移除任意位置的，所以需要记录，并在获取时抵消</remarks>
		public TreeDictionary<long, int> removeIdDictionary;

		private int traversalCount;

		public void Clear()
		{
			nodeRuleQueue?.Clear();
			nodeIdHash?.Clear();
			removeIdDictionary?.Clear();
			traversalCount = 0;
		}

		public void Remove(long id)
		{
			if (nodeIdHash != null && nodeIdHash.Contains(id))
			{
				nodeIdHash.Remove(id);

				//累计强制移除的节点id
				removeIdDictionary ??= this.AddChild(out removeIdDictionary);
				if (removeIdDictionary.TryGetValue(id, out var count))
				{
					removeIdDictionary[id] = count + 1;
				}
				else
				{
					removeIdDictionary.Add(id, 1);
				}
			}
		}

		public void Remove(INode node) => Remove(node.Id);

		public bool TryAdd(INode node, RuleList ruleList)
		{
			//节点存在则不允许重复添加。
			//如果节点是意外回收了，那么Id是递增的不再出现，也就是那个回收的Id已经被永久销毁了，同样禁止添加。
			if (nodeIdHash != null && nodeIdHash.Contains(node.Id)) return false;
			nodeIdHash ??= this.AddChild(out nodeIdHash);
			nodeRuleQueue ??= this.AddChild(out nodeRuleQueue);
			NodeRef<INode> NodeRef = new(node);
			nodeRuleQueue.Enqueue((NodeRef, ruleList));
			nodeIdHash.Add(node.Id);
			return true;
		}

		public IEnumerator<ValueTuple<INode, RuleList>> GetEnumerator()
		{
			traversalCount = nodeRuleQueue is null ? 0 : nodeRuleQueue.Count;
			for (int i = 0; i < traversalCount; i++)
			{
				//从队列里拿到id
				if (nodeRuleQueue.TryDequeue(out (NodeRef<INode>, RuleList) nodeRuleTuple))
				{
					while (true)
					{
						long id = nodeRuleTuple.Item1.nodeId;
						//假如id被主动移除了
						if (removeIdDictionary != null && removeIdDictionary.TryGetValue(id, out int count))
						{
							removeIdDictionary[id] = --count;//回收次数抵消
							if (count == 0) removeIdDictionary.Remove(id);//次数为0时删除id
							if (removeIdDictionary.Count == 0)//假如字典空了,则释放
							{
								removeIdDictionary.Dispose();
								removeIdDictionary = null;
							}

							if (traversalCount != 0) traversalCount--; //遍历数抵消
																	   //获取下一个id,假如队列空了,则直接返回退出
							if (!nodeRuleQueue.TryDequeue(out nodeRuleTuple)) yield break;
						}
						else
						{
							INode node = nodeRuleTuple.Item1.Value;

							if (node == null)//节点意外回收
							{
								//字典移除节点Id，节点回收后id改变了，而id是递增，绝对不会再出现的。
								nodeIdHash.Remove(id);

								if (traversalCount != 0) traversalCount--; //遍历数抵消
																		   //获取下一个id,假如队列空了,则直接返回退出
								if (!nodeRuleQueue.TryDequeue(out nodeRuleTuple)) yield break;
							}
							else//节点存在
							{
								nodeRuleQueue.Enqueue(nodeRuleTuple);//塞回队列用于下次遍历
								yield return (node, nodeRuleTuple.Item2);//返回执行组
								break;
							}
						}
					}
				}
			}
		}
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	public static class RuleActuatorBaseRule
	{
		class RemoveRule : RemoveRule<RuleActuatorBase>
		{
			protected override void OnEvent(RuleActuatorBase self)
			{
				self.Clear();
			}
		}
	}
}
