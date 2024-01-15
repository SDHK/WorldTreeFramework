
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/24 10:52

* 描述： 实体绑定Mono工具

*/

using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
//using UnityEditor.AddressableAssets;
using UnityEngine;

namespace EditorTool
{
    //[CreateAssetMenu]
    /// <summary>
    /// 实体绑定Mono工具
    /// </summary>
    [FilePath("Assets/WorldTreeFramework/UnityEnvironment/EditorTool/EditorToolKitWindow/ToolPages/EntityBindMono/Assets/EntityBindMonoTool.asset")]

    public class EntityBindMonoTool :ScriptableSingleton<EntityBindMonoTool>
    {

        [BoxGroup("路径")]
        [FolderPath(RequireExistingPath = true)]
        [LabelText("生成脚本路径"), LabelWidth(100)]
        public string CreateFilePath;

        [ShowIf("@Update()")]
        [LabelText("分组列表")]
        [Searchable]
        [ListDrawerSettings(Expanded = true, CustomAddFunction = "AddButton", CustomRemoveElementFunction = "RemoveButton")]
        public List<ObjectBindGroup> groups = new List<ObjectBindGroup>();

        public ObjectBindGroup AddButton()
        {
            return new ObjectBindGroup();
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
                AddList(item);
            }
            return true;
        }

        private void AddList(ObjectBindGroup group)
        {
            if (group.addMonoObjects.Count > 0)
            {
                foreach (var monoObject in group.addMonoObjects)
                {
                    if (!groups.Any(item => item.objects.Any((item) => item.monoObject == monoObject || item.monoObject.name == monoObject.name)))
                    {
                        //AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry(
                        //    AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(monoObject.gameObject)).ToString()
                        //    , AddressableAssetSettingsDefaultObject.Settings.DefaultGroup).SetAddress(monoObject.gameObject.name);

                        group.objects.Add(new ObjectBindItem() { monoObject = monoObject});
                    }
                    else
                    {
                        Debug.Log($"{monoObject.gameObject.name} 已存在");
                    }
                }
                group.addMonoObjects.Clear();
            }
        }
    }

}