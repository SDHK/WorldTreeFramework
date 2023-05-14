using UnityEngine;

namespace WorldTree
{
    public class GUIBeginHorizontal : GUIBase
        , AsRule<IRecycleRule>
    {
        public void Draw()
        {
            GUILayout.BeginHorizontal(Style, options);
        }
    }

    class GUIBeginHorizontalRecycleSystem : RecycleRule<GUIBeginHorizontal>
    {
        public override void OnEvent(GUIBeginHorizontal self)
        {
            //self.PoolRecycle(self.style);
            self.style = null;
        }
    }
}
