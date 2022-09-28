
/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/28 11:46

* 描述： 世界树框架编辑器窗口

*/

using UnityEditor;
using UnityEngine;

namespace WorldTree
{
    /// <summary>
    /// 世界树框架编辑器窗口
    /// </summary>
    public class WorldTreeFrameworkWindow : Entity
    {
        public static EntityManager root;

        [MenuItem("WorldTree/Window")]
        static void ShowWindow()
        {
            if (root == null)
            {
                root = new EntityManager();
                World.Log = Debug.Log;
                World.LogWarning = Debug.LogWarning;
                World.LogError = Debug.LogError;
            }
            root.AddComponent<WorldTreeFrameworkWindow>();
        }

        public SystemGlobalBroadcast<IEnableSystem> enable;
        public SystemGlobalBroadcast<IDisableSystem> disable;
        public SystemGlobalBroadcast<IUpdateSystem> update;
        public SystemGlobalBroadcast<IOnGUISystem> onGUI;

        public EditorHomePage HomePage;
    }

    class WorldTreeFrameworkWindowAddSystem : AddSystem<WorldTreeFrameworkWindow>
    {
        public override void OnAdd(WorldTreeFrameworkWindow self)
        {
            self.AddComponent<EditorGUIWindow>();
            self.enable = self.GetSystemBroadcast<IEnableSystem>();
            self.update = self.GetSystemBroadcast<IUpdateSystem>();
            self.disable = self.GetSystemBroadcast<IDisableSystem>();
            self.onGUI = self.GetSystemBroadcast<IOnGUISystem>();
            self.AddComponent<EditorHomePage>();
        }
    }
    class WorldTreeFrameworkWindowGUIDrawSystem : GUIDrawSystem<WorldTreeFrameworkWindow>
    {
        public override void DrawGUI(WorldTreeFrameworkWindow self)
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
