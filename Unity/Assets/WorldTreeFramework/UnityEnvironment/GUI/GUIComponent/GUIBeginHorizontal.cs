using UnityEngine;

namespace WorldTree
{
    public class GUIBeginHorizontal : GUIBase
    {
        public void Draw()
        {
            GUILayout.BeginHorizontal(Style, options);
        }
    }

    class GUIBeginHorizontalRecycleSystem : RecycleRule<GUIBeginHorizontal>
    {
        protected override void OnEvent(GUIBeginHorizontal self)
        {
            //self.PoolRecycle(self.style);
            self.style = null;
        }
    }
}
