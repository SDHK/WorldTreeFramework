
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/3 15:41

* 描述： 实体活跃
* 
* 模仿 Unity GameObject树 的 活跃功能

*/

using System.Linq;

namespace WorldTree
{
    public abstract partial class Entity
    {
        /// <summary>
        /// 活跃开关
        /// </summary>
        private bool activeToggle = false;

        /// <summary>
        /// 活跃状态
        /// </summary>
        private bool active = false;

        /// <summary>
        /// 活跃事件标记
        /// </summary>
        public bool activeEventMark = false;

        /// <summary>
        /// 活跃状态
        /// </summary>
        public bool IsActive => active;

        /// <summary>
        /// 活跃标记
        /// </summary>
        public bool ActiveToggle => activeToggle;

        /// <summary>
        /// 设置激活状态
        /// </summary>
        public void SetActive(bool value)
        {
            if (activeToggle != value)
            {
                activeToggle = value;

                if (active != ((Parent == null) ? activeToggle : Parent.active && activeToggle))
                {
                    RefreshActive();
                }
            }
        }

        /// <summary>
        /// 刷新激活状态：层序遍历设置子节点
        /// </summary>
        private void RefreshActive()
        {
            UnitQueue<Entity> queue = Root.ObjectPoolManager.Get<UnitQueue<Entity>>();
            queue.Enqueue(this);
            while (queue.Any())
            {
                var current = queue.Dequeue();
                if (current.active != ((current.Parent == null) ? current.activeToggle : current.Parent.active && current.activeToggle))
                {
                    current.active = !current.active;

                    if (current.components != null)
                    {
                        foreach (var item in current.components)
                        {
                            queue.Enqueue(item.Value);
                        }
                    }

                    if (current.children != null)
                    {
                        foreach (var item in current.children)
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
