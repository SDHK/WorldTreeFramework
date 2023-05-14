
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/29 16:17

* 描述： 

*/

using UnityEngine;

namespace WorldTree
{
    public class GUIBox : GUIBase
        , AsRule<IAwakeRule>
        , AsRule<IRecycleRule>
    {
        public void Draw()
        {
            GUILayout.Box(text, Style, options);
        }
    }

    class GUIBoxRecycleSystem : RecycleRule<GUIBox>
    {
        public override void OnEvent(GUIBox self)
        {
            //self.PoolRecycle(self.style);
            self.style = null;
        }
    }
}
