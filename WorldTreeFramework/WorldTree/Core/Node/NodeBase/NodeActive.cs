
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/3 15:41

* 描述： 节点活跃
* 
* 模仿 Unity GameObject树 的 活跃功能

*/

namespace WorldTree
{
    public abstract partial class Node
    {
        /// <summary>
        /// 活跃开关
        /// </summary>
        public bool m_ActiveToggle = false;

        /// <summary>
        /// 活跃状态
        /// </summary>
        public bool m_Active = false;

        /// <summary>
        /// 活跃事件标记
        /// </summary>
        public bool m_ActiveEventMark = false;

        /// <summary>
        /// 活跃状态
        /// </summary>
        public bool IsActive => m_Active;

        /// <summary>
        /// 活跃标记
        /// </summary>
        public bool ActiveToggle => m_ActiveToggle;
    }
}
