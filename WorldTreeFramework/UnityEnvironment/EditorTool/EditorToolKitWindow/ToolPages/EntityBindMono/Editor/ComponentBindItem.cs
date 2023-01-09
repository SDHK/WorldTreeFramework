
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/30 17:33

* 描述： 组件绑定项

*/

using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EditorTool
{
    [Serializable]
    public class ComponentBindItem
    {

        [HideInInspector]
        public bool IsShow = true;

        [HorizontalGroup("A", width: 300)]

        [GUIColor(1, 1, 0)]
        [HideLabel]
        [ReadOnly]
        public Component component;

        [HorizontalGroup("A")]
        [HideLabel]
        public string comment;

        [GUIColor(0, 1, 0)]
        [ShowIf("@!IsShow&&eventTags.Count>0")]
        [HorizontalGroup("A", width: 100)]
        [Button("事件绑定", ButtonSizes.Medium)]
        public void FoldShow()
        {
            IsShow = true;
        }
        [GUIColor(1, 1, 0)]
        [ShowIf("@IsShow&&eventTags.Count>0")]
        [HorizontalGroup("A", width: 100)]
        [Button("事件绑定", ButtonSizes.Medium)]
        public void FoldHide()
        {
            IsShow = false;
        }

        [ShowIf("IsShow")]
        [TableList(HideToolbar = true, AlwaysExpanded = true, ShowIndexLabels = true, IsReadOnly = true)]
        public List<ComponentEventItem> eventTags = new List<ComponentEventItem>();


        public void Refresh()
        {
            eventTags.Clear();

            if (Script.EventNames.TryGetValue(component.GetType(), out string[] names))
            {
                foreach (var name in names)
                {
                    eventTags.Add(new ComponentEventItem() { eventName = name });
                }
            }
        }
    }

}
