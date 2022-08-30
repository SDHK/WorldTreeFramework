/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/29 21:26

* 描述： 

*/
using System;
using UnityEngine;

namespace WorldTree
{
    public class GUIFoldoutButton : GUIBase
    {
        public Action action;
        public bool isFoldout;

        public bool Draw()
        {
            if (GUILayout.Button(text, Style, options))
            {
                isFoldout = !isFoldout;
                action?.Invoke();
            }
            return isFoldout;
        }

        public bool Draw(bool value)
        {
            this.isFoldout = value;
            if (GUILayout.Button(text, Style, options))
            {
                isFoldout = !isFoldout;
                action?.Invoke();
            }
            return isFoldout;
        }
    }
    class GUIFoldoutButtonRecycleSystem : RecycleSystem<GUIFoldoutButton>
    {
        public override void OnRecycle(GUIFoldoutButton self)
        {
            self.Root.ObjectPoolManager.Recycle(self.style);
            self.style = null;
        }
    }
}
