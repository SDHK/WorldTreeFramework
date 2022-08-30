/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/29 21:23

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
    public class GUIBeginScrollView : GUIBase
    {
        /// <summary>
        /// 滚动位置
        /// </summary>
        public Vector2 scrollPosition;
        public  void Draw()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, Style, options);
        }
    }

    class GUIBeginScrollViewRecycleSystem : RecycleSystem<GUIBeginScrollView>
    {
        public override void OnRecycle(GUIBeginScrollView self)
        {
            self.Root.ObjectPoolManager.Recycle(self.style);
            self.style = null;
        }
    }
}
