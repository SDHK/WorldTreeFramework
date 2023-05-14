
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/29 16:18

* 描述： 

*/

using System;
using UnityEngine;

namespace WorldTree
{
    public class GUIButton : GUIBase
    {
        public Action action;

        public  void Draw()
        {
            if (GUILayout.Button(text, Style, options))
            {
                action?.Invoke();
            }
        }
    }

    class GUIButtonRecycleSystem : RecycleRule<GUIButton>
    {
        public override void OnEvent(GUIButton self)
        {
            //self.PoolRecycle(self.style);
            self.style = null;
        }
    }
}
