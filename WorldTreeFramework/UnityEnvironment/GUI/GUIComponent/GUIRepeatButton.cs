using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WorldTree
{
    public class GUIRepeatButton : GUIBase
    {
        public Action action;

        public void Draw()
        {
            if (GUILayout.RepeatButton(text, Style, options))
            {
                action?.Invoke();
            }
        }
    }

    class GUIRepeatButtonRecycleSystem : RecycleSystem<GUIRepeatButton>
    {
        public override void OnRecycle(GUIRepeatButton self)
        {
            self.PoolRecycle(self.style);
            self.style = null;
        }
    }
}
