
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/30 17:35

* 描述： 组件事件绑定项

*/

using Sirenix.OdinInspector;
using System;

namespace WorldTree
{
    [Serializable]
    public class ComponentEventItem
    {
        [ReadOnly]
        [HideLabel, HorizontalGroup("事件回调"), TableColumnWidth(500)]
        public string eventName;
        [HideLabel, HorizontalGroup("是否生成"), TableColumnWidth(100)]
        public bool bit = true;
    }
}
