
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/24 10:52

* 描述： 实体绑定Mono工具

*/

using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorTool
{
    /// <summary>
    /// 实体绑定Mono工具
    /// </summary>
    [CreateAssetMenu]
    public class EntityBindMonoTool : ScriptableObject
    {

        [BoxGroup("路径")]
        [FolderPath(RequireExistingPath = true)]
        [LabelText("生成脚本路径"), LabelWidth(100)]
        public string CreateFilePath;


        [ShowIf("@Update()")]
        [LabelText("分组")]
        [Searchable]
        [ListDrawerSettings(Expanded = true, CustomAddFunction = "AddButton",CustomRemoveElementFunction ="RemoveButton")]
        public List<ObjectBindGroup> groups = new List<ObjectBindGroup>();

        public ObjectBindGroup AddButton()
        {
            return new ObjectBindGroup() { monoBindEntityTool = this };
        }
        public bool RemoveButton(ObjectBindGroup objectBindGroup)
        {
            if (EditorUtility.DisplayDialog($"删除组 {objectBindGroup.groupName} ", $"确定要删除 {objectBindGroup.groupName} 组吗？", "✔", "❌"))
            {
                objectBindGroup.RemoveGroup();
                groups.Remove(objectBindGroup);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool Update()
        {
            foreach (var item in groups)
            {
                item.UpdateRefresh();
            }
            return true;
        }
    }

}