
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/29 16:17

* 描述： 

*/

using UnityEngine;

namespace WorldTree
{
    public class GUIBox : GUIBase
    {
        public void Draw()
        {
            GUILayout.Box(text, Style, options);
        }
    }

    class GUIBoxRecycleSystem : RecycleSystem<GUIBox>
    {
        public override void OnRecycle(GUIBox self)
        {
            //self.PoolRecycle(self.style);
            self.style = null;
        }
    }
}
