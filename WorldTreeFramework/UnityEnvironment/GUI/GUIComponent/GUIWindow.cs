/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/29 21:46

* 描述： 

*/
using UnityEngine;


//思考self.Log方法

namespace WorldTree
{
    public class GUIWindow : GUIBase
    {
        public Rect rect = new Rect(0, 0, 400, 300);
        public bool isDrag = true;

        public void Window(int id)
        {
            if (isDrag)
            {
                GUI.DragWindow();
            }
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
