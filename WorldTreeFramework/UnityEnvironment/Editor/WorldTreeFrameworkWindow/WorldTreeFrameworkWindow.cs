
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
        
       
        , AsRule<IGUIDrawSystem>
        , AsRule<IEditorWindowInspectorUpdateSystem>

    {
        public static WorldTreeCore root;

        [MenuItem("WorldTree/Window")]
        static void ShowWindow()
        {
            //if (Core == null)
            //{
            //    Core = new WorldTreeCore();
            //    World.Log = Debug.Log;
            //    World.LogWarning = Debug.LogWarning;
            //    World.LogError = Debug.LogError;
            //}
            //Core.AddComponent<WorldTreeFrameworkWindow>();
        }

        public IRuleActuator<IEnableRule> enable;
        public IRuleActuator<IDisableRule> disable;
        public IRuleActuator<IUpdateRule> update;
        public IRuleActuator<IGuiUpdateRule> onGUI;

        public EditorHomePage HomePage;
    }

    class WorldTreeFrameworkWindowAddSystem : AddRule<WorldTreeFrameworkWindow>
    {
        public override void OnEvent(WorldTreeFrameworkWindow self)
        {
            self.AddComponent(out EditorGUIWindow _);
            self.enable = self.GetGlobalNodeRuleActuator<IEnableRule>();
            self.update = self.GetGlobalNodeRuleActuator<IUpdateRule>();
            self.disable = self.GetGlobalNodeRuleActuator<IDisableRule>();
            self.onGUI = self.GetGlobalNodeRuleActuator<IGuiUpdateRule>();
            self.AddComponent(out EditorHomePage _);
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
        public override void OnEvent(WorldTreeFrameworkWindow self)
        {
            self.enable.Send();
            self.update.Send(0.02f);
            self.disable.Send();
        }
    }

}
