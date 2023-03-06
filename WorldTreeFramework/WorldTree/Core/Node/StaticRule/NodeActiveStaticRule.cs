
/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/6 14:16

* 描述： 

*/

namespace WorldTree
{
    public static class NodeActiveStaticRule
    {

        /// <summary>
        /// 设置激活状态
        /// </summary>
        public static void SetActive(this Node self, bool value)
        {
            if (self.m_ActiveToggle != value)
            {
                self.m_ActiveToggle = value;

                if (self.m_Active != ((self.Parent == null) ? self.m_ActiveToggle : self.Parent.m_Active && self.m_ActiveToggle))
                {
                    self.RefreshActive();
                }
            }
        }

        /// <summary>
        /// 刷新激活状态：层序遍历设置子节点
        /// </summary>
        public static void RefreshActive(this Node self)
        {
            UnitQueue<Node> queue = (self.Root.NodePoolManager.IsDisposed) ? new UnitQueue<Node>() : self.Root.PoolGet<UnitQueue<Node>>();
            queue.Enqueue(self);
            while (queue.Count != 0)
            {
                var current = queue.Dequeue();
                if (current.m_Active != ((current.Parent == null) ? current.m_ActiveToggle : current.Parent.m_Active && current.m_ActiveToggle))
                {
                    current.m_Active = !current.m_Active;

                    if (current.m_Components != null)
                    {
                        foreach (var item in current.m_Components)
                        {
                            queue.Enqueue(item.Value);
                        }
                    }

                    if (current.m_Children != null)
                    {
                        foreach (var item in current.m_Children)
                        {
                            queue.Enqueue(item.Value);
                        }
                    }
                }
            }
            queue.Dispose();
        }

    }
}
