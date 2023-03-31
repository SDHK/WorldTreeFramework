/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/24 11:33

* 描述： 树节点值类型基类

*/

namespace WorldTree
{
    /// <summary>
    /// 树节点值类型基类
    /// </summary>
    public abstract class TreeValue : Node, ChildOf<INode>
    {
        ///// <summary>
        ///// 全局法则类型 暂不考虑
        ///// </summary>
        //public Type RuleType;

        /// <summary>
        /// 观察字典 【监听绑定父级】
        /// </summary>
        public TreeDictionary<long, TreeValue> m_ObservableDictionary;

        /// <summary>
        /// 监听者字典 【监听绑定子级】
        /// </summary>
        public TreeDictionary<long, TreeValue> m_ListenerDictionary;
    }




    public static class TreeValueRule
    {
        class TreeValueRemoveRule : RemoveRule<TreeValue>
        {
            public override void OnEvent(TreeValue self)
            {
                if (self.m_ListenerDictionary != null)//从绑定子级移除自己
                {
                    foreach (var nodeKV in self.m_ListenerDictionary)
                    {
                        nodeKV.Value.m_ObservableDictionary?.Remove(self.Id);
                        if (nodeKV.Value.m_ObservableDictionary.Count == 0)
                        {
                            nodeKV.Value.m_ObservableDictionary.Dispose();
                            nodeKV.Value.m_ObservableDictionary = null;
                        }
                    }
                }

                if (self.m_ObservableDictionary != null)//从绑定父级移除自己
                {
                    foreach (var nodeKV in self.m_ObservableDictionary)
                    {
                        nodeKV.Value.m_ListenerDictionary?.Remove(self.Id);
                        if (nodeKV.Value.m_ListenerDictionary.Count == 0)
                        {
                            nodeKV.Value.m_ListenerDictionary.Dispose();
                            nodeKV.Value.m_ListenerDictionary = null;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 单向绑定
        /// </summary>
        public static void Bind<TV, TV1>(this TV self, TV1 treeValue)
            where TV : TreeValue
            where TV1 : TV
        {
            _ = self.m_ListenerDictionary ?? self.AddChild(out self.m_ListenerDictionary);
            _ = treeValue.m_ObservableDictionary ?? treeValue.AddChild(out treeValue.m_ObservableDictionary);

            self.m_ListenerDictionary.Add(treeValue.Id, treeValue);
            treeValue.m_ObservableDictionary.TryAdd(self.Id, self);
        }

        /// <summary>
        /// 双向绑定
        /// </summary>
        public static void BindTwoWay<TV, TV1>(this TV self, TV1 treeValue)
            where TV : TreeValue
            where TV1 : TV
        {
            _ = self.m_ListenerDictionary ?? self.AddChild(out self.m_ListenerDictionary);
            _ = treeValue.m_ListenerDictionary ?? treeValue.AddChild(out treeValue.m_ListenerDictionary);

            _ = self.m_ObservableDictionary ?? self.AddChild(out self.m_ObservableDictionary);
            _ = treeValue.m_ObservableDictionary ?? treeValue.AddChild(out treeValue.m_ObservableDictionary);


            self.m_ListenerDictionary.TryAdd(treeValue.Id, treeValue);
            treeValue.m_ObservableDictionary.TryAdd(self.Id, self);

            treeValue.m_ListenerDictionary.TryAdd(self.Id, self);
            self.m_ObservableDictionary.TryAdd(treeValue.Id, treeValue);
        }

    }

}
