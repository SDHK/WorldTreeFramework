using System;
using UnityEngine;

namespace WorldTree
{
    public class GUIRepeatButton : GUIBase
        , AsRule<IRecycleRule>
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
        public override void OnEvent(GUIRepeatButton self)
        {
            //self.PoolRecycle(self.style);
            self.style = null;
        }
    }
}
