/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 16:21

* 描述： 动态节点队列

*/

namespace WorldTree
{
    /// <summary>
    /// 动态节点队列
    /// </summary>
    public class DynamicNodeQueue : Node
    {
        /// <summary>
        /// 节点id队列
        /// </summary>
        public UnitQueue<long> idQueue;

        /// <summary>
        /// 节点id被移除的次数
        /// </summary>
        public UnitDictionary<long, int> removeIdDictionary;

        /// <summary>
        /// 节点名单
        /// </summary>
        public UnitDictionary<long, Node> nodeDictionary;


        /// <summary>
        /// 当前队列数量
        /// </summary>
        public int Count => idQueue.Count;

        /// <summary>
        /// 节点入列实体
        /// </summary>
        public void Enqueue(Node node)
        {
            if (nodeDictionary.ContainsKey(node.id)) return;
            idQueue.Enqueue(node.id);
            nodeDictionary.Add(node.id, node);
        }

        /// <summary>
        /// 节点移除
        /// </summary>
        public void Remove(Node node)
        {
            if (nodeDictionary.ContainsKey(node.id))
            {
                nodeDictionary.Remove(node.id);

                //累计强制移除的节点id
                if (removeIdDictionary.TryGetValue(node.id, out var count))
                {
                    removeIdDictionary[node.id] = count + 1;
                }
                else
                {
                    removeIdDictionary.Add(node.id, 1);
                }
            }
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            nodeDictionary.Clear();
            removeIdDictionary.Clear();
            idQueue.Clear();
        }


        /// <summary>
        /// 获取队顶
        /// </summary>
        public Node Peek()
        {
            if (TryPeek(out Node node))
            {
                return node;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 尝试获取队顶
        /// </summary>
        public bool TryPeek(out Node node)
        {
            do
            {
                //尝试获取一个id
                if (idQueue.TryPeek(out long id))
                {
                    //假如id被回收了
                    if (removeIdDictionary.TryGetValue(id, out int count))
                    {
                        //回收次数抵消
                        removeIdDictionary[id] = --count;
                        //次数为0时删除id
                        if (count == 0) removeIdDictionary.Remove(id);
                        //移除这个id
                        idQueue.Dequeue();
                    }
                    else
                    {
                        return nodeDictionary.TryGetValue(id, out node);
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
        public Node Dequeue()
        {
            if (TryDequeue(out Node node))
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
        public bool TryDequeue(out Node node)
        {
            //尝试获取一个id
            if (idQueue.TryDequeue(out long id))
            {
                //假如id被回收了
                while (removeIdDictionary.TryGetValue(id, out int count))
                {
                    //回收次数抵消
                    removeIdDictionary[id] = --count;

                    //次数为0时删除id
                    if (count == 0) removeIdDictionary.Remove(id);

                    //获取下一个id
                    if (!idQueue.TryDequeue(out id))
                    {
                        //假如队列空了,则直接返回退出
                        node = null;
                        return false;
                    }
                }
                //此时的id是正常id
                if (nodeDictionary.TryGetValue(id, out node))
                {
                    nodeDictionary.Remove(node.id);
                    return true;
                }
            }

            node = null;
            return false;
        }
    }

    class DynamicNodeQueueAddSystem : AddRule<DynamicNodeQueue>
    {
        public override void OnEvent(DynamicNodeQueue self)
        {
            self.PoolGet(out self.idQueue);
            self.PoolGet(out self.removeIdDictionary);
            self.PoolGet(out self.nodeDictionary);
        }
    }

    class DynamicNodeQueueRemoveSystem : RemoveRule<DynamicNodeQueue>
    {
        public override void OnEvent(DynamicNodeQueue self)
        {
            self.idQueue.Dispose();
            self.removeIdDictionary.Dispose();
            self.nodeDictionary.Dispose();
        }
    }
}
