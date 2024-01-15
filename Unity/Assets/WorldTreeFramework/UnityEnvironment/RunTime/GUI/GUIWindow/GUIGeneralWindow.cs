﻿
/****************************************

* 作者： 闪电黑客
* 日期： 2022/9/21 10:11

* 描述： GUI通用窗体

*/

using UnityEngine;

namespace WorldTree
{
    public class GUIGeneralWindow : Node
        , AsRule<IGUIDrawSystem>
    {
        public IRuleGroup<IGUIDrawSystem> group;

        public GUIBeginVertical VerticalBox;

        public GUIBeginHorizontal HorizontalBox;


    }

    class GUIGeneralWindowAddSystem : AddRule<GUIGeneralWindow>
    {
        protected override void OnEvent(GUIGeneralWindow self)
        {
            //self.group = self.Core.RuleManager.GetRuleGroup<IGUIDrawSystem>();
            self.AddComponent(out GUIWindow _);
        }
    }

    class GUIGeneralWindowOnGUISystem : GUIDrawSystem<GUIGeneralWindow>
    {
        protected override void OnEvent(GUIGeneralWindow self)
        {
            self.VerticalBox.Draw();

            self.group?.Send(self.Parent);

            GUILayout.EndVertical();

        }
    }
}