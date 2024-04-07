using System;
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

    class GUIRepeatButtonRecycleSystem : RecycleRule<GUIRepeatButton>
    {
        protected override void Execute(GUIRepeatButton self)
        {
            //self.PoolRecycle(self.style);
            self.style = null;
        }
    }
}
