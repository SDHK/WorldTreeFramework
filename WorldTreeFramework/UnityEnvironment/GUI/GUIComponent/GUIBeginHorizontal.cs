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
            //self.PoolRecycle(self.style);
            self.style = null;
        }
    }
}
