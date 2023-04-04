
/****************************************

* 作者： 闪电黑客
* 日期： 2023/4/4 14:33

* 描述： 

*/

namespace WorldTree
{
    class DynamicNodeQueueAddRule : AddRule<DynamicNodeQueue>
    {
        public override void OnEvent(DynamicNodeQueue self)
        {
            self.AddChild(out self.idQueue);
            self.AddChild(out self.removeIdDictionary);
            self.AddChild(out self.nodeDictionary);
        }
    }
    class DynamicNodeQueueReferencedRemoveRule : ReferencedRemoveRule<DynamicNodeQueue>
    {
        public override void OnEvent(DynamicNodeQueue self, INode node)
        {
            self.Remove(node);
        }
    }

    class DynamicNodeQueueRemoveRule : RemoveRule<DynamicNodeQueue>
    {
        public override void OnEvent(DynamicNodeQueue self)
        {
            self.idQueue = default;
            self.removeIdDictionary = default;
            self.nodeDictionary = default;
        }
    }

    public static class DynamicNodeQueueRule
    {
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
            self.nodeDictionary.Clear();
            self.removeIdDictionary.Clear();
            self.idQueue.Clear();
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
                    return true;
                }
            }

            node = null;
            return false;
        }
    }
}
