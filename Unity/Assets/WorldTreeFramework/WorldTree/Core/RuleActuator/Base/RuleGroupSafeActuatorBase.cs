/****************************************

* 作者： 闪电黑客
* 日期： 2023/12/07 07:30:49

* 描述： 法则集合执行器基类
*
* 执行拥有指定法则的节点

*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace WorldTree
{
	public abstract class RuleGroupSafeActuatorBase : Node, IRuleActuator, IRuleActuatorEnumerable
	{
		/// <summary>
		/// 单法则集合
		/// </summary>
		public RuleGroup ruleGroup;

		/// <summary>
		/// 节点id队列
		/// </summary>
		public TreeQueue<long> idQueue;

		/// <summary>
		/// 节点字典
		/// </summary>
		/// <remarks>确保同一时刻不允许有两个相同的节点遍历</remarks>
		public TreeDictionary<long, NodeRef<INode>> Nodes;

		/// <summary>
		/// 节点id被移除的次数
		/// </summary>
		/// <remarks>队列是无法移除任意位置的，所以需要记录，并在获取时抵消</remarks>
		public TreeDictionary<long, int> removeIdDictionary;


		private int traversalCount;


		public void Clear() => Nodes.Clear();

		public void Remove(long id)
		{
			if (Nodes != null && Nodes.ContainsKey(id))
			{
				Nodes.Remove(id);

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

				if (Nodes.Count == 0)
				{
					Clear();
				}
			}
		}

		public void Remove(INode node) => Remove(node.Id);

		public bool TryAdd(INode node)
		{
			if (Nodes.TryGetValue(node.Id, out var nodeRef))
			{
				if (nodeRef.Value != null) return false;//假如节点存在，并且补上意外回收,则禁止添加
				Remove(node.Id);//假如节点被意外回收了
			}
			idQueue.Enqueue(node.Id);
			Nodes.Add(node.Id, new(node));
			return true;
		}

		public IEnumerator<ValueTuple<INode, RuleList>> GetEnumerator()
		{
			traversalCount = idQueue is null ? 0 : idQueue.Count;
			for (int i = 0; i < traversalCount; i++)
			{
				//从队列里拿到id
				if (idQueue.TryDequeue(out long id))
				{
					while (true)
					{
						//假如id被主动移除了
						if (removeIdDictionary != null && removeIdDictionary.TryGetValue(id, out int count))
						{
							if (traversalCount != 0) traversalCount--; //遍历数抵消

							removeIdDictionary[id] = --count;//回收次数抵消
							if (count == 0) removeIdDictionary.Remove(id);//次数为0时删除id
							if (removeIdDictionary.Count == 0)//假如字典空了,则释放
							{
								removeIdDictionary.Dispose();
								removeIdDictionary = null;
							}
							if (!idQueue.TryDequeue(out id)) yield break;//获取下一个id,假如队列空了,则直接返回退出
						}
						//节点不存在
						else if (!Nodes.TryGetValue(id, out NodeRef<INode> nodeRef))
						{
							if (traversalCount != 0) traversalCount--; //遍历数抵消
							if (!idQueue.TryDequeue(out id)) yield break;//获取下一个id,假如队列空了,则直接返回退出
						}
						//假如节点存在
						else
						{
							INode node = nodeRef.Value;
							//假如节点被意外回收了 或 法则不存在
							if (node == null || !ruleGroup.TryGetValue(node.Type, out RuleList ruleList))
							{
								Nodes.Remove(id);//字典移除节点
								if (traversalCount != 0) traversalCount--; //遍历数抵消
								if (!idQueue.TryDequeue(out id)) yield break;//获取下一个id,假如队列空了,则直接返回退出
							}
							else
							{
								idQueue.Enqueue(id);
								yield return (node, ruleList);
								break;
							}
						}
					}

				}
			}
		}
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
