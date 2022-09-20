/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/29 21:46

* 描述： 

*/
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;


//思考self.Log方法

//GUIDrawSystem绘制UI


namespace WorldTree
{
    
    public class GUIWindow : GUIBase
    {
        public Rect rect = new Rect(0, 0, 400, 300);
        public bool isDrag = true;
        public SystemGroup group;

        public void Window(int id)
        {
            group?.Send(Parent);
            if (isDrag)
            {
                GUI.DragWindow();
            }
        }
    }

    class GUIWindowNewSystem : NewSystem<GUIWindow>
    {
        public override void OnNew(GUIWindow self)
        {
            self.group = self.GetSystemGroup<IGUIWindowSystem>();
        }
    }

    class GUIWindowOnGUISystem : OnGUISystem<GUIWindow>
    {
        public override void OnGUI(GUIWindow self, float deltaTime)
        {
            self.rect = GUILayout.Window(self.GetHashCode(), self.rect, self.Window, default(string), self.Style);
        }
    }

    class GUIWindowRecycleSystem : RecycleSystem<GUIWindow>
    {
        public override void OnRecycle(GUIWindow self)
        {
            self.Root.ObjectPoolManager.Recycle(self.style);
            self.style = null;
        }
    }


}
