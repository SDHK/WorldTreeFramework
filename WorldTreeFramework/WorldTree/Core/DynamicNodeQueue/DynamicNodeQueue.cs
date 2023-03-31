/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 16:21

* 描述： 动态节点队列
* 
* 主要为了可以按照顺序遍历的同时可随机移除内容

*/

using System.Collections;

namespace WorldTree
{


    /// <summary>
    /// 动态节点队列
    /// </summary>
    public class DynamicNodeQueue : Node, ComponentOf<INode>, ChildOf<INode>
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
        public int Count => nodeDictionary.Count;

     

        /// <summary>
        /// 节点入列
        /// </summary>
        public void Enqueue(INode node)
        {
            if (nodeDictionary.ContainsKey(node.Id)) return;
            idQueue.Enqueue(node.Id);
            nodeDictionary.Add(node.Id, node);
        }

        /// <summary>
        /// 节点移除
        /// </summary>
        public void Remove(INode node)
        {
            if (nodeDictionary.ContainsKey(node.Id))
            {
                nodeDictionary.Remove(node.Id);

                //累计强制移除的节点id
                if (removeIdDictionary.TryGetValue(node.Id, out var count))
                {
                    removeIdDictionary[node.Id] = count + 1;
                }
                else
                {
                    removeIdDictionary.Add(node.Id, 1);
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
        public INode Peek()
        {
            if (TryPeek(out INode node))
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
        public bool TryPeek(out INode node)
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
        public INode Dequeue()
        {
            if (TryDequeue(out INode node))
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
        public bool TryDequeue(out INode node)
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
                    nodeDictionary.Remove(node.Id);
                    return true;
                }
            }

            node = null;
            return false;
        }
    }

    class DynamicNodeQueueAddRule : AddRule<DynamicNodeQueue>
    {
        public override void OnEvent(DynamicNodeQueue self)
        {
            self.AddChild(out self.idQueue);
            self.AddChild(out self.removeIdDictionary);
            self.AddChild(out self.nodeDictionary);
        }
    }

    class DynamicNodeQueueRemoveRule : RemoveRule<DynamicNodeQueue>
    {
        public override void OnEvent(DynamicNodeQueue self)
        {
            self.idQueue.Dispose();
            self.removeIdDictionary.Dispose();
            self.nodeDictionary.Dispose();
        }
    }
}
