/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/29 21:46

* 描述： 

*/
using UnityEngine;


//思考self.Log方法

//GUIDrawSystem绘制UI


namespace WorldTree
{

    public class GUIWindow : GUIBase, ComponentOf<GUIGeneralWindow>
        , AsRule<IAwakeRule>
        , AsRule<IGuiUpdateRule>
    {
        public Rect rect = new Rect(0, 0, 400, 300);
        public bool isDrag = true;
        public IRuleGroup<IGUIDrawSystem> group;

        public void Window(int id)
        {
            group?.Send(Parent);
            if (isDrag)
            {
                GUI.DragWindow();
            }
        }
    }

    class GUIWindowAddSystem : AddRule<GUIWindow>
    {
        protected override void OnEvent(GUIWindow self)
        {
            //self.group = self.Core.RuleManager.GetRuleGroup<IGUIDrawSystem>();
        }
    }

    class GUIWindowOnGUISystem : GuiUpdateRule<GUIWindow>
    {
        protected override void OnEvent(GUIWindow self, float deltaTime)
        {
            self.rect = GUILayout.Window(self.GetHashCode(), self.rect, self.Window, default(string), self.Style);
        }
    }

    class GUIWindowRecycleSystem : RecycleRule<GUIWindow>
    {
        protected override void OnEvent(GUIWindow self)
        {
            //self.PoolRecycle(self.style);
            self.style = null;
        }
    }


}
