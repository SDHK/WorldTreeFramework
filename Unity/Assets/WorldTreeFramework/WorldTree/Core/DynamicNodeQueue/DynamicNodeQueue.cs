/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 16:21

* 描述： 动态节点队列
* 
* 主要为了可以按照顺序遍历的同时可随机移除内容

*/

namespace WorldTree
{
    /// <summary>
    /// 动态节点队列
    /// </summary>
    public class DynamicNodeQueue : Node, ComponentOf<INode>, ChildOf<INode>
        , AsRule<IAwakeRule>
    {
        /// <summary>
        /// 节点id队列
        /// </summary>
        public TreeQueue<long> idQueue;

        /// <summary>
        /// 节点id被移除的次数
        /// </summary>
        public TreeDictionary<long, int> removeIdDictionary;

        /// <summary>
        /// 节点名单
        /// </summary>
        public TreeDictionary<long, INode> nodeDictionary;

        /// <summary>
        /// 当前队列数量
        /// </summary>
        public int Count => nodeDictionary is null ? 0 : nodeDictionary.Count;

        /// <summary>
        /// 动态的遍历数量
        /// </summary>
        /// <remarks>当遍历时移除后，在发生抵消的时候减少数量</remarks>
        public int traversalCount;
    }

    public static class DynamicNodeQueueRule
    {
        class AddRule : AddRule<DynamicNodeQueue>
        {
            protected override void OnEvent(DynamicNodeQueue self)
            {
                self.AddChild(out self.idQueue);
                self.AddChild(out self.removeIdDictionary);
                self.AddChild(out self.nodeDictionary);
            }
        }


        class RemoveRule : RemoveRule<DynamicNodeQueue>
        {
            protected override void OnEvent(DynamicNodeQueue self)
            {
                self.idQueue = default;
                self.removeIdDictionary = default;
                self.nodeDictionary = default;
            }
        }

        class ReferencedChildRemoveRule : ReferencedChildRemoveRule<DynamicNodeQueue>
        {
            protected override void OnEvent(DynamicNodeQueue self, INode node)
            {
                self.Remove(node);
            }
        }


        /// <summary>
        /// 节点入列并建立引用关系
        /// </summary>
        public static void EnqueueReferenced(this DynamicNodeQueue self, INode node)
        {
            if (self.nodeDictionary.ContainsKey(node.Id)) return;
            self.Referenced(node);

            self.idQueue.Enqueue(node.Id);
            self.nodeDictionary.Add(node.Id, node);
        }

        /// <summary>
        /// 节点入列
        /// </summary>
        public static void Enqueue(this DynamicNodeQueue self, INode node)
        {
            if (self.nodeDictionary.ContainsKey(node.Id)) return;
            self.idQueue.Enqueue(node.Id);
            self.nodeDictionary.Add(node.Id, node);
        }

        /// <summary>
        /// 节点移除
        /// </summary>
        public static void Remove(this DynamicNodeQueue self, INode node)
        {
            if (self.nodeDictionary.ContainsKey(node.Id))
            {
                self.nodeDictionary.Remove(node.Id);
                self.DeReferenced(node);
                //累计强制移除的节点id
                if (self.removeIdDictionary.TryGetValue(node.Id, out var count))
                {
                    self.removeIdDictionary[node.Id] = count + 1;
                }
                else
                {
                    self.removeIdDictionary.Add(node.Id, 1);
                }
            }
        }

        /// <summary>
        /// 清除
        /// </summary>
        public static void Clear(this DynamicNodeQueue self)
        {
            if (self.nodeDictionary != null)
            {
                foreach (var item in self.nodeDictionary)
                {
                    self.DeReferenced(item.Value);
                }
                self.nodeDictionary?.Clear();
            }
            self.removeIdDictionary?.Clear();
            self.idQueue?.Clear();
        }

        /// <summary>
        /// 获取队顶
        /// </summary>
        public static INode Peek(this DynamicNodeQueue self)
        {
            if (self.TryPeek(out INode node))
            {
                return node;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 刷新动态遍历数量
        /// </summary>
        public static void RefreshTraversalCount(this DynamicNodeQueue self)
        {
            self.traversalCount = self.idQueue is null ? 0 : self.idQueue.Count;
        }


        /// <summary>
        /// 尝试获取队顶
        /// </summary>
        public static bool TryPeek(this DynamicNodeQueue self, out INode node)
        {
            do
            {
                //尝试获取一个id
                if (self.idQueue.TryPeek(out long id))
                {
                    //假如id被回收了
                    if (self.removeIdDictionary.TryGetValue(id, out int count))
                    {
                        //回收次数抵消
                        self.removeIdDictionary[id] = --count;
                        if (self.traversalCount > 0) self.traversalCount--;

                        //次数为0时删除id
                        if (count == 0) self.removeIdDictionary.Remove(id);
                        //移除这个id
                        self.idQueue.Dequeue();
                    }
                    else
                    {
                        return self.nodeDictionary.TryGetValue(id, out node);
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
        public static INode Dequeue(this DynamicNodeQueue self)
        {
            if (self.TryDequeue(out INode node))
            {
                return node;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 尝试出列
        /// </summary>
        public static bool TryDequeue(this DynamicNodeQueue self, out INode node)
        {
            //尝试获取一个id
            if (self.idQueue.TryDequeue(out long id))
            {
                //假如id被回收了
                while (self.removeIdDictionary.TryGetValue(id, out int count))
                {
                    //回收次数抵消
                    self.removeIdDictionary[id] = --count;
                    if (self.traversalCount > 0) self.traversalCount--;

                    //次数为0时删除id
                    if (count == 0) self.removeIdDictionary.Remove(id);

                    //获取下一个id
                    if (!self.idQueue.TryDequeue(out id))
                    {
                        //假如队列空了,则直接返回退出
                        node = null;
                        return false;
                    }
                }
                //此时的id是正常id
                if (self.nodeDictionary.TryGetValue(id, out node))
                {
                    self.nodeDictionary.Remove(node.Id);
                    self.DeReferenced(node);
                    return true;
                }
            }

            node = null;
            return false;
        }
    }
}
