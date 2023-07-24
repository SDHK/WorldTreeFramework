
/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/6 14:16

* 描述： 节点活跃
* 
* 模仿 Unity GameObject树 的 活跃功能
* 
*/

namespace WorldTree
{
    public static class NodeActiveRule
    {

        /// <summary>
        /// 设置激活状态
        /// </summary>
        public static void SetActive(this INode self, bool value)
        {
            if (self.ActiveToggle != value)
            {
                self.ActiveToggle = value;

                if (self.IsActive != ((self.Parent == null) ? self.ActiveToggle : self.Parent.IsActive && self.ActiveToggle))
                {
                    self.RefreshActive();
                }
            }
        }

        /// <summary>
        /// 刷新激活状态：层序遍历设置子节点
        /// </summary>
        public static void RefreshActive(this INode self)
        {
            using (self.PoolGet(out UnitQueue<INode> queue))
            {
                queue.Enqueue(self);
                while (queue.Count != 0)
                {
                    var current = queue.Dequeue();
                    if (current.IsActive != ((current.Parent == null) ? current.ActiveToggle : current.Parent.IsActive && current.ActiveToggle))
                    {
                        current.IsActive = !current.IsActive;
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
            }
        }

    }
}
