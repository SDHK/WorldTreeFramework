
/****************************************

* 作者： 闪电黑客
* 日期： 2022/10/11 15:53

* 描述： 工具箱窗口

*/


using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace EditorTool
{
    public class ToolKitWindow : OdinMenuEditorWindow
    {
        public static string rootPath = "Assets/SDHK/WorldTreeFramework/UnityEnvironment/EditorTool/EditorToolKitWindow/ToolPages/";

        //public EntityManager root;

        //public SystemGlobalBroadcast<IEnableSystem> enable;
        //public SystemGlobalBroadcast<IDisableSystem> disable;
        //public SystemGlobalBroadcast<IUpdateSystem> update;
        //public SystemGlobalBroadcast<IOnGUISystem> onGUI;

        [MenuItem("WorldTree/ToolKit")]
        public static void OpenFrameEditor()
        {
            var window = GetWindow<ToolKitWindow>();
            window.titleContent = new GUIContent("工具箱");
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1000, 700);
        }

        protected override void Initialize()
        {
            //if (root != null) root.Dispose();

            //root = new EntityManager();

            //World.Log = Debug.Log;
            //World.LogWarning = Debug.LogWarning;
            //World.LogError = Debug.LogError;
            //enable = root.GetSystemGlobalBroadcast<IEnableSystem>();
            //update = root.GetSystemGlobalBroadcast<IUpdateSystem>();
            //disable = root.GetSystemGlobalBroadcast<IDisableSystem>();
            //onGUI = root.GetSystemGlobalBroadcast<IOnGUISystem>();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(true);
            tree.AddAssetAtPath("关于", GuidePage.Inst.FilePath).AddIcon(EditorIcons.Info);
            tree.AddAssetAtPath("脚本工具", ScriptInitSetting.Inst.FilePath).AddIcon(EditorIcons.SingleUser);
            tree.AddAssetAtPath("资源编辑器", ScriptableEditor.Inst.FilePath);
            tree.AddAssetAtPath("实体绑定Mono", EntityBindMonoTool.Inst.FilePath);

            return tree;
        }

        private void OnInspectorUpdate()
        {
            //enable.Send();
            //update.Send(0.02f);
            //disable.Send();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            //enable = null;
            //update = null;
            //disable = null;
            //onGUI = null;
            //root.Dispose();
            //root = null;
        }
        protected override void OnBeginDrawEditors()
        {
            base.OnBeginDrawEditors();

            var selected = MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = MenuTree.Config.SearchToolbarHeight;


            //SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            //{

            //}
            //SirenixEditorGUI.EndHorizontalToolbar();

        }

        protected override void OnEndDrawEditors()
        {
            //onGUI.Send(0.02f);
        }
    }
}
