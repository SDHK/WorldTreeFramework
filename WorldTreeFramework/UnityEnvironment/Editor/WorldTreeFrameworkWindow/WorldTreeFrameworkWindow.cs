
/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/28 11:46

* 描述： 世界树框架编辑器驱动窗口

*/

using UnityEditor;
using UnityEngine;

namespace WorldTree
{
    /// <summary>
    /// 世界树框架编辑器驱动窗口
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

        public SystemGlobalBroadcast enable;
        public SystemGlobalBroadcast disable;
        public SystemGlobalBroadcast update;
        public SystemGlobalBroadcast onGUI;

        public EditorHomePage HomePage;
    }

    class WorldTreeFrameworkWindowAddSystem : AddSystem<WorldTreeFrameworkWindow>
    {
        public override void OnAdd(WorldTreeFrameworkWindow self)
        {
            self.AddComponent<EditorGUIWindow>();
            self.enable = self.GetSystemGlobalBroadcast<IEnableSystem>();
            self.update = self.GetSystemGlobalBroadcast<IUpdateSystem>();
            self.disable = self.GetSystemGlobalBroadcast<IDisableSystem>();
            self.onGUI = self.GetSystemGlobalBroadcast<IOnGUISystem>();
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
