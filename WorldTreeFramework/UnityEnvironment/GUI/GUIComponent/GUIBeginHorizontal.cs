using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WorldTree
{
    public class GUIBeginHorizontal : GUIBase
    {
        public  void Draw()
        {
            GUILayout.BeginHorizontal(Style, options);
        }
    }

    class GUIBeginHorizontalRecycleSystem : RecycleSystem<GUIBeginHorizontal>
    {
        public override void OnRecycle(GUIBeginHorizontal self)
        {
            self.Root.ObjectPoolManager.Recycle(self.style);
            self.style = null;
        }
    }
}
