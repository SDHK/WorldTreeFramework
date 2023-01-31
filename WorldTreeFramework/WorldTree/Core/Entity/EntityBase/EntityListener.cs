
/****************************************

* 作者： 闪电黑客
* 日期： 2023/1/31 19:08

* 描述： 监听器标记
* 
* 用于实体动态监听全局任意实体事件

*/

using System;

namespace WorldTree
{
    public abstract partial class Entity
    {
        /// <summary>
        /// 监听器标记
        /// </summary>
        public bool isListener = false;
        /// <summary>
        /// 监听目标类型
        /// </summary>
        public Type ListenerTarget; ///还有接口怎么处理？
    }
}
