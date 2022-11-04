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
        public void Draw()
        {
            GUILayout.BeginVertical(Style, options);
        }
    }

    class GUIBeginVerticalAddSystem : AddSystem<GUIBeginVertical>
    {
        public override void OnAdd(GUIBeginVertical self)
        {
            self.Texture = self.GetColorTexture(0.1f);

            self.Padding = new RectOffset(5, 5, 5, 5);
        }
    }

    class GUIBeginVerticalRecycleSystem : RecycleSystem<GUIBeginVertical>
    {
        public override void OnRecycle(GUIBeginVertical self)
        {
            self.PoolRecycle(self.style);
            self.style = null;
        }
    }
}
