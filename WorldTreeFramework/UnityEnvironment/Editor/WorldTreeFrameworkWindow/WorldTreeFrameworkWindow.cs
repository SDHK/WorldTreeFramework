
/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/28 11:46

* 描述： 世界树框架编辑器驱动窗口

*/

using UnityEditor;
using UnityEngine;
using WorldTree;

namespace EditorTool
{
    /// <summary>
    /// 世界树框架编辑器驱动窗口
    /// </summary>
    public class WorldTreeFrameworkWindow : Node
    {
        public static WorldTreeRoot root;

        [MenuItem("WorldTree/Window")]
        static void ShowWindow()
        {
            //if (root == null)
            //{
            //    root = new WorldTreeRoot();
            //    World.Log = Debug.Log;
            //    World.LogWarning = Debug.LogWarning;
            //    World.LogError = Debug.LogError;
            //}
            //root.AddComponent<WorldTreeFrameworkWindow>();
        }

        public RuleActuator enable;
        public RuleActuator disable;
        public RuleActuator update;
        public RuleActuator onGUI;

        public EditorHomePage HomePage;
    }

    class WorldTreeFrameworkWindowAddSystem : AddRule<WorldTreeFrameworkWindow>
    {
        public override void OnEvent(WorldTreeFrameworkWindow self)
        {
            self.AddComponent<EditorGUIWindow>();
            self.enable = self.GetGlobalNodeRuleActuator<IEnableRule>();
            self.update = self.GetGlobalNodeRuleActuator<IUpdateRule>();
            self.disable = self.GetGlobalNodeRuleActuator<IDisableRule>();
            self.onGUI = self.GetGlobalNodeRuleActuator<IGuiUpdateRule>();
            self.AddComponent<EditorHomePage>();
        }
    }
    class WorldTreeFrameworkWindowGUIDrawSystem : GUIDrawSystem<WorldTreeFrameworkWindow>
    {
        public override void OnEvent(WorldTreeFrameworkWindow self)
        {
            self.onGUI.Send(0.02f);
        }
    }

    class WorldTreeFrameworkWindowEditorWindowInspectorUpdateSystem : EditorWindowInspectorUpdateSystem<WorldTreeFrameworkWindow>
    {
        public override void OnInspectorUpdate(WorldTreeFrameworkWindow self)
        {
            self.enable.Send();
            self.update.Send(0.02f);
            self.disable.Send();
        }
    }

}
