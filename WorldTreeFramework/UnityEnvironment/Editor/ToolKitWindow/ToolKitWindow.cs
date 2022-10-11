
/****************************************

* 作者： 闪电黑客
* 日期： 2022/10/11 15:53

* 描述： 工具箱窗口

*/

using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace WorldTree
{
    public class ToolKitWindow : OdinMenuEditorWindow
    {
        public static string rootPath = "Assets/SDHKTool/WorldTreeFramework/UnityEnvironment/Editor/ToolKitWindow/";

        [MenuItem("WorldTree/ToolKit")]
        public static void OpenFrameEditor()
        {
            var window = GetWindow<ToolKitWindow>();
            window.titleContent = new GUIContent("工具箱");
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1000, 700);
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(true);
            tree.AddAssetAtPath("关于", rootPath + "GuidePage/Page/Page.asset").AddIcon(EditorIcons.Info);
            tree.AddAssetAtPath("脚本对象编辑", rootPath + "ScriptableObjectEditor/Page/Page.asset");
            return tree;
        }
        protected override void OnBeginDrawEditors()
        {
            if (SirenixEditorGUI.ToolbarButton(new GUIContent("新增模块")))
            {


            }

        }
    }
}
