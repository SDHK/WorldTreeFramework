
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/29 16:18

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

    class GUIButtonRecycleSystem : RecycleSystem<GUIButton>
    {
        public override void OnRecycle(GUIButton self)
        {
            self.Root.ObjectPoolManager.Recycle(self.style);
            self.style = null;
        }
    }
}
