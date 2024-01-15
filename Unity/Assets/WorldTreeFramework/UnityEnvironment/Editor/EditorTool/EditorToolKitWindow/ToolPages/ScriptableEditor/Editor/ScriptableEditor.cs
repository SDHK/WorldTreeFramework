
/****************************************

* 作者： 闪电黑客
* 日期： 2022/10/17 9:40

* 描述： Scriptable Object编辑器工具

*/

using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace EditorTool
{
    /// <summary>
    /// ScriptableObject编辑器工具
    /// </summary>

    [FilePath("Assets/WorldTreeFramework/UnityEnvironment/EditorTool/EditorToolKitWindow/ToolPages/ScriptableEditor/Assets/ScriptableEditor.asset")]
    public class ScriptableEditor : ScriptableSingleton<ScriptableEditor>
    {
        [BoxGroup("路径")]
        [FolderPath(RequireExistingPath = true)]
        [LabelText("脚本文件夹"), LabelWidth(100)]
        public string CreateFilePath;
        [BoxGroup("路径")]
        [FolderPath(RequireExistingPath = true)]
        [LabelText("资源文件夹"), LabelWidth(100)]
        public string CreateAssetPath;

        [BoxGroup("编辑")]
        [LabelText("命名空间"), LabelWidth(60)]
        public string nameSpace = "WorldTree";

        [InfoBox("命名冲突", InfoMessageType.Error, "@this.IsRepeatClassName()")]
        [BoxGroup("编辑")]
        [LabelText("新建类名"), LabelWidth(60)]
        public string ClassName;
       

        [BoxGroup("编辑")]
        [Button("添加", ButtonSizes.Large)]
        public void AddClass()
        {
            if (!IsRepeatClassName() && ClassName != "")
            {
                classes.Add(new EditorAssetClass() { ClassName = ClassName});
                ClassName = "";
            }
        }

        public bool IsRepeatClassName()
        {
            return classes.Any((item) => item.ClassName == ClassName);
        }

        [LabelText("列表")]
        [Searchable]
        [ListDrawerSettings(ShowPaging = true,Expanded = true, HideAddButton = true, CustomRemoveElementFunction = "RemoveClassButton")]
        public List<EditorAssetClass> classes = new List<EditorAssetClass>();


        public void RemoveClassButton(EditorAssetClass assetClass)
        {
            if (EditorUtility.DisplayDialog($"删除类型 {assetClass.ClassName} ", $"确定要删除 {assetClass.ClassName} 类吗？", "✔", "❌"))
            {
                assetClass.DeleteListClass();
                assetClass.DeleteClass();
                classes.Remove(assetClass);
            }
        }
    }

}
