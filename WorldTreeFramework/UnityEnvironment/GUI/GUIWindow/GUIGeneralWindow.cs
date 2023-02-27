
/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/21 10:11

* 描述： GUI通用窗体

*/

using UnityEngine;

namespace WorldTree
{
    public class GUIGeneralWindow : Entity
    {
        public SystemGroup group;

        public GUIBeginVertical VerticalBox;

        public GUIBeginHorizontal HorizontalBox;


    }

    class GUIGeneralWindowAddSystem : AddSystem<GUIGeneralWindow>
    {
        public override void OnEvent(GUIGeneralWindow self)
        {
            self.group = self.GetSystemGroup<IGUIDrawSystem>();
            self.AddComponent<GUIWindow>();
        }
    }

    class GUIGeneralWindowOnGUISystem : GUIDrawSystem<GUIGeneralWindow>
    {
        public override void DrawGUI(GUIGeneralWindow self)
        {
            self .VerticalBox.Draw();

            self.group?.Send(self.Parent);

            GUILayout.EndVertical();

        }
    }
}
