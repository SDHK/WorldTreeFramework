
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/29 14:15

* 描述： 对象绑定组

*/

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using WorldTree;

namespace EditorTool
{
    [Serializable]
    public class ObjectBindGroup
    {
        [HideInInspector]
        public bool IsShow = true;

        [HideLabel, HorizontalGroup("A", width: 200)]
        public string groupName;

        [GUIColor(0, 1, 1)]
        [LabelText("拖拽添加对象")]
        [HorizontalGroup("A")]
        [ListDrawerSettings(Expanded = false, HideAddButton = true, ShowItemCount = false)]
        public List<MonoObject> addMonoObjects = new List<MonoObject>();

        [GUIColor(0, 1, 0)]
        [ShowIf("@objects.Count > 0 &&!IsShow")]
        [HorizontalGroup("A", width: 100)]
        [Button("对象列表", ButtonSizes.Medium)]
        public void FoldShow()
        {
            IsShow = true;
        }
        [GUIColor(1, 1, 0)]
        [ShowIf("@objects.Count > 0&&IsShow")]
        [HorizontalGroup("A", width: 100)]
        [Button("对象列表", ButtonSizes.Medium)]
        public void FoldHide()
        {
            IsShow = false;
        }


        [ShowIf("@objects.Count > 0&&IsShow")]
        [LabelText("对象列表")]
        [Searchable]
        [ListDrawerSettings(Expanded = true, HideAddButton = true, CustomRemoveElementFunction = "RemoveButton")]
        public List<ObjectBindItem> objects = new List<ObjectBindItem>();



        public void UpdateRefresh()
        {
            //检测删除Null组件，同时提供Update刷新
            for (int i = 0; i < objects.Count;)
            {
                if (objects[i].monoObject != null)
                {
                    objects[i].UpdateRefresh();
                    i++;
                }
                else
                {
                    objects.RemoveAt(i);
                }
            }
        }

        public bool RemoveButton(ObjectBindItem objectBindItem)
        {
            if (EditorUtility.DisplayDialog($"删除类型 {objectBindItem.monoObject.gameObject.name} ", $"确定要删除 {objectBindItem.monoObject.gameObject.name} 类吗？", "✔", "❌"))
            {
                objects.Remove(objectBindItem);
                objectBindItem.DeleteBindScript();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void RemoveGroup()
        {
            foreach (var item in objects)
            {
                item.DeleteBindScript();
            }
            objects.Clear();
        }

    }



}
