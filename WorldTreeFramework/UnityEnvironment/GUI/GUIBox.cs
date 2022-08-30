
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/29 16:17

* 描述： 

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WorldTree
{
    public class GUIBox : GUIBase
    {
        public  void Draw()
        {
            GUILayout.Box(text, Style, options);
        }
    }


    class GUIBoxNewSystem : NewSystem<GUIBox>
    {
        public override void OnNew(GUIBox self)
        {
        }
    }

    class GUIBoxRecycleSystem : RecycleSystem<GUIBox>
    {
        public override void OnRecycle(GUIBox self)
        {
            self.Root.ObjectPoolManager.Recycle(self.style);
            self.style = null;
        }
    }

    class GUIBoxDestroySystem : DestroySystem<GUIBox>
    {
        public override void OnDestroy(GUIBox self)
        {
        }
    }
}
