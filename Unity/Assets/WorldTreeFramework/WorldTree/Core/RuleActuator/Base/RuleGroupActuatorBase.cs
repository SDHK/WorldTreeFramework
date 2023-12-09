/****************************************

* 作者： 闪电黑客
* 日期： 2023/6/1 19:44

* 描述： 法则集合执行器基类
*
* 执行拥有指定法则的节点

 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace WorldTree
{

	/// <summary>
	/// 法则执行器基类
	/// </summary>
	public abstract class RuleGroupActuatorBase : Node, IRuleActuator, IRuleActuatorEnumerable
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
		public TreeDictionary<long, INode> nodeDictionary;

		/// <summary>
		/// 节点id被移除的次数
		/// </summary>
		public TreeDictionary<long, int> removeIdDictionary;

		private int traversalCount;

		public override string ToString()
		{
			return $"RuleGroupActuator : {ruleGroup?.RuleType.HashCore64ToType()}";
		}

		public IEnumerator<ValueTuple<INode, RuleList>> GetEnumerator()
		{
			traversalCount = idQueue is null ? 0 : idQueue.Count;
			for (int i = 0; i < traversalCount; i++)
			{
				if (idQueue.TryDequeue(out long id))
				{
					while (true)
					{
						//假如id被回收了
						if (removeIdDictionary != null && removeIdDictionary.TryGetValue(id, out int count))
						{
							//回收次数抵消
							removeIdDictionary[id] = --count;

							//次数为0时删除id
							if (count == 0) removeIdDictionary.Remove(id);
							if (removeIdDictionary.Count == 0)
							{
								removeIdDictionary.Dispose();
								removeIdDictionary = null;
							}
							if (!idQueue.TryDequeue(out id)) yield break;//获取下一个id,假如队列空了,则直接返回退出
						}
						else if (nodeDictionary.TryGetValue(id, out INode node))
						{
							if (ruleGroup.TryGetValue(node.Type, out RuleList ruleList))
							{
								//id压回队列
								idQueue.Enqueue(id);
								yield return (node, ruleList);
							}
						}
					}
					
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


		/// <summary>
		/// 尝试出列
		/// </summary>
		private bool TryGetNext(out INode node, out RuleList ruleList)
		{
			//尝试获取一个id
			if (idQueue != null && idQueue.TryDequeue(out long id))
			{
				//假如id被回收了
				while (removeIdDictionary != null && removeIdDictionary.TryGetValue(id, out int count))
				{
					//回收次数抵消
					removeIdDictionary[id] = --count;
					//遍历数抵消
					if (traversalCount != 0) traversalCount--;

					//次数为0时删除id
					if (count == 0) removeIdDictionary.Remove(id);
					if (removeIdDictionary.Count == 0)
					{
						removeIdDictionary.Dispose();
						removeIdDictionary = null;
					}

					//获取下一个id
					if (!idQueue.TryDequeue(out id))
					{
						//假如队列空了,则直接返回退出
						ruleList = null;
						node = null;
						return false;
					}
				}

				//此时的id是正常id，查找节点是否存在
				if (nodeDictionary.TryGetValue(id, out node))
				{
					this.ruleGroup.TryGetValue(node.Type, out ruleList);
					//id压回队列
					idQueue.Enqueue(id);
					return true;
				}
				else
				{
					idQueue.Enqueue(id);
				}
			}
			ruleList = null;
			node = null;
			return false;
		}

		#region 添加

		/// <summary>
		/// 尝试添加节点
		/// </summary>
		public bool TryAdd(INode node)
		{
			nodeDictionary ??= this.AddChild(out nodeDictionary);
			if (nodeDictionary.ContainsKey(node.Id))
			{
				return false;
			}

			idQueue ??= this.AddChild(out idQueue);
			nodeDictionary ??= this.AddChild(out nodeDictionary);

			idQueue.Enqueue(node.Id);
			nodeDictionary.Add(node.Id, node);

			return true;
		}


		#endregion

		#region 移除

		/// <summary>
		/// 移除节点
		/// </summary>
		public void Remove(long id)
		{
			if (nodeDictionary != null && nodeDictionary.TryGetValue(id, out INode node))
			{
				nodeDictionary.Remove(id);

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

				if (nodeDictionary.Count == 0)
				{
					Clear();
				}
			}
		}

		/// <summary>
		/// 移除节点
		/// </summary>
		public void Remove(INode node)
		{
			Remove(node.Id);
		}

		/// <summary>
		/// 清除
		/// </summary>
		public void Clear()
		{
			if (nodeDictionary != null)
			{
				foreach (INode node in nodeDictionary.Values)
				{
					this.DeReferenced(node);
				}
				nodeDictionary.Dispose();
				nodeDictionary = null;
			}

			idQueue?.Dispose();
			removeIdDictionary?.Dispose();

			idQueue = null;
			removeIdDictionary = null;

			traversalCount = 0;
		}

		#endregion

	}

	public static class RuleActuatorBaseRule
	{
		class RemoveRule : RemoveRule<RuleGroupActuatorBase>
		{
			protected override void OnEvent(RuleGroupActuatorBase self)
			{
				self.ruleGroup = null;
				self.Clear();
			}
		}
	}


}
