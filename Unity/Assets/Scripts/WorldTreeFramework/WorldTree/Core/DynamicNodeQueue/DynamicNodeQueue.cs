/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 16:21

* 描述： 动态节点队列
* 
* 主要为了可以按照顺序遍历的同时可随机移除内容

*/

using System.Collections;
using System.Collections.Generic;

namespace WorldTree
{

	/// <summary>
	/// 动态节点队列
	/// </summary>
	public class DynamicNodeQueue : Node, ComponentOf<INode>, ChildOf<INode>, IEnumerable<INode>
		, AsChildBranch
		, AsAwake
	{
		/// <summary>
		/// 节点id队列
		/// </summary>
		public TreeQueue<NodeRef<INode>> nodeQueue;

		/// <summary>
		/// 节点id被移除的次数
		/// </summary>
		public TreeDictionary<long, int> removeIdDict;

		/// <summary>
		/// 节点Id字典
		/// </summary>
		public TreeHashSet<long> nodeIdHash;

		/// <summary>
		/// 当前队列数量
		/// </summary>
		public int Count => nodeQueue.Count;

		/// <summary>
		/// 动态的遍历数量
		/// </summary>
		/// <remarks>当遍历时移除后，在发生抵消的时候减少数量</remarks>
		private int traversalCount;


		/// <summary>
		/// 节点入列
		/// </summary>
		public bool TryEnqueue(INode node)
		{
			if (nodeIdHash != null && nodeIdHash.Contains(node.InstanceId)) return false;
			nodeQueue ??= this.AddChild(out nodeQueue);
			nodeIdHash ??= this.AddChild(out nodeIdHash);
			NodeRef<INode> nodeRef = new(node, false);
			this.nodeQueue.Enqueue(nodeRef);
			this.nodeIdHash.Add(node.InstanceId);
			return true;
		}

		/// <summary>
		/// 节点移除
		/// </summary>
		public void Remove(INode node) => Remove(node.InstanceId);

		/// <summary>
		/// 节点移除
		/// </summary>
		public void Remove(long id)
		{
			if (nodeIdHash != null && nodeIdHash.Contains(id))
			{
				nodeIdHash.Remove(id);

				//累计强制移除的节点id
				removeIdDict ??= this.AddChild(out removeIdDict);
				if (removeIdDict.TryGetValue(id, out var count))
				{
					removeIdDict[id] = count + 1;
				}
				else
				{
					removeIdDict.Add(id, 1);
				}
			}
		}

		/// <summary>
		/// 清除
		/// </summary>
		public void Clear()
		{
			nodeQueue?.Clear();
			nodeIdHash?.Clear();
			removeIdDict?.Clear();
			traversalCount = 0;
		}

		/// <summary>
		/// 获取队顶
		/// </summary>
		public INode Peek() => TryPeek(out INode node) ? node : null;

		/// <summary>
		/// 尝试获取队顶
		/// </summary>
		public bool TryPeek(out INode node)
		{
			do
			{
				//尝试获取一个id
				if (nodeQueue != null && nodeQueue.TryPeek(out NodeRef<INode> nodeRef))
				{
					long id = ((NodeRef<INode>)null).InstanceId;
					//假如id被回收了
					if (removeIdDict != null && removeIdDict.TryGetValue(id, out int count))
					{
						//回收次数抵消
						removeIdDict[id] = --count;
						if (count == 0) removeIdDict.Remove(id);//次数为0时删除id
						if (removeIdDict.Count == 0)//假如字典空了,则释放
						{
							removeIdDict.Dispose();
							removeIdDict = null;
						}

						if (traversalCount > 0) traversalCount--;
						nodeQueue.Dequeue();//移除
					}
					else
					{
						node = ((NodeRef<INode>)null).Value;
						if (node == null)//节点意外回收
						{
							//字典移除节点InstanceId，节点回收后id改变了，而id是递增，绝对不会再出现的。
							nodeIdHash.Remove(InstanceId);
							if (traversalCount != 0) traversalCount--; //遍历数抵消
							nodeQueue.Dequeue();//移除
						}
						else
						{
							return true;
						}
					}
				}
				else
				{
					node = null;
					return false;
				}

			} while (true);
		}
		/// <summary>
		/// 节点出列
		/// </summary>
		public INode Dequeue() => TryDequeue(out INode node) ? node : null;


		/// <summary>
		/// 尝试出列
		/// </summary>
		public bool TryDequeue(out INode node)
		{
			//尝试获取一个id
			if (nodeQueue != null && nodeQueue.TryDequeue(out NodeRef<INode> nodeRef))
			{
				while (true)
				{
					long id = nodeRef.InstanceId;
					//假如id被主动移除了
					if (removeIdDict != null && removeIdDict.TryGetValue(id, out int count))
					{
						id = nodeRef.InstanceId;

						removeIdDict[id] = --count;//回收次数抵消
						if (count == 0) removeIdDict.Remove(id);//次数为0时删除id
						if (removeIdDict.Count == 0)//假如字典空了,则释放
						{
							removeIdDict.Dispose();
							removeIdDict = null;
						}

						if (traversalCount > 0) traversalCount--;

						//获取下一个id
						if (!nodeQueue.TryDequeue(out nodeRef))
						{
							//假如队列空了,则直接返回退出
							node = null;
							return false;
						}
					}
					else
					{
						node = nodeRef.Value;
						if (node == null)//节点意外回收
						{
							//字典移除节点InstanceId，节点回收后id改变了，而id是递增，绝对不会再出现的。
							nodeIdHash.Remove(id);

							if (traversalCount != 0) traversalCount--;
							//获取下一个id
							if (!nodeQueue.TryDequeue(out nodeRef))
							{
								//假如队列空了,则直接返回退出
								node = null;
								return false;
							}
						}
						else //节点存在
						{
							return true;
						}
					}
				}
			}
			node = null;
			return false;
		}

		public IEnumerator<INode> GetEnumerator()
		{
			traversalCount = nodeQueue is null ? 0 : nodeQueue.Count;
			for (int i = 0; i < traversalCount; i++)
			{
				if (TryDequeue(out INode node)) yield return node;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	public static class DynamicNodeQueueRule
	{
		class RemoveRule : RemoveRule<DynamicNodeQueue>
		{
			protected override void Execute(DynamicNodeQueue self)
			{
				self.Clear();
				self.nodeQueue = null;
				self.removeIdDict = null;
				self.nodeIdHash = null;
			}
		}
	}
}
