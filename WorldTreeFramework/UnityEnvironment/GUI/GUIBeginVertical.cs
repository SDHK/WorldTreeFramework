using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WorldTree
{
    public class GUIBeginVertical : GUIBase
    {
        public  void Draw()
        {
            GUILayout.BeginVertical(Style, options);
        }
    }

    class GUIBeginVerticalRecycleSystem : RecycleSystem<GUIBeginVertical>
    {
        public override void OnRecycle(GUIBeginVertical self)
        {
            self.Root.ObjectPoolManager.Recycle(self.style);
            self.style = null;
        }
    }
}
