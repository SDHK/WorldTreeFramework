/****************************************

* 作者： 闪电黑客
* 日期： 2023/12/07 02:39:36

* 描述： 法则列表执行器基类
*
* 填装节点的法则执行

*/

using System;
using System.Collections;
using System.Collections.Generic;

namespace WorldTree
{
	public abstract class RuleListActuatorBase : Node, IRuleActuator, IRuleActuatorEnumerable
	{
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

		/// <summary>
		/// 法则集合字典
		/// </summary>
		public TreeDictionary<long, RuleList> ruleGroupDictionary;


		private int traversalCount;

		public IEnumerator<ValueTuple<INode, RuleList>> GetEnumerator()
		{
			traversalCount = idQueue is null ? 0 : idQueue.Count;
			for (int i = 0; i < traversalCount; i++)
			{
				if (TryGetNext(out INode node, out RuleList ruleList))
				{
					yield return (node, ruleList);
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
					//存在则获取列表
					ruleGroupDictionary.TryGetValue(id, out ruleList);
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

		/// <summary>
		/// 尝试添加节点，并建立引用关系
		/// </summary>
		public bool TryAddReferenced(INode node)
		{
			if (TryAdd(node))
			{
				this.Referenced(node);
				return true;
			}
			return false;
		}


		/// <summary>
		/// 尝试添加节点与对应法则
		/// </summary>
		public bool TryAdd(INode node, RuleList ruleList)
		{

			if (ruleList != null)
			{
				if (TryAdd(node))
				{
					ruleGroupDictionary ??= this.AddChild(out ruleGroupDictionary);
					ruleGroupDictionary.Add(node.Id, ruleList);
					return true;
				}
				else
				{
					if (ruleGroupDictionary.TryGetValue(node.Id, out RuleList OldRuleGroup))
					{
						if (OldRuleGroup.RuleType != ruleList.RuleType) World.LogError("执行器中已存在这个节点，但注册的法则不同。");
					}
					return false;
				}
			}
			else
			{
				World.LogError("法则为空");
				return false;
			}
		}

		/// <summary>
		/// 尝试添加节点与对应法则，并建立引用关系
		/// </summary>
		public bool TryAddReferenced(INode node, RuleList ruleList)
		{
			if (TryAdd(node, ruleList))
			{
				this.Referenced(node);
				return true;
			}
			return false;
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
				ruleGroupDictionary?.Remove(id);

				this.DeReferenced(node);
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
			ruleGroupDictionary?.Dispose();
			removeIdDictionary?.Dispose();

			idQueue = null;
			ruleGroupDictionary = null;
			removeIdDictionary = null;

			traversalCount = 0;
		}




		#endregion

	}

	public static class RuleListActuatorBaseRule
	{
		class RemoveRule : RemoveRule<RuleListActuatorBase>
		{
			protected override void OnEvent(RuleListActuatorBase self)
			{
				self.Clear();
			}
		}
	}
}
