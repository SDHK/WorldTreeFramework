
/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/6 17:47

* 描述： 

*/

using System;

namespace WorldTree
{
    public static partial class  TreeValueRule
    {
        /// <summary>
        /// 做渐变
        /// </summary>
        public static TreeTween<T> DoTween<T>(this TreeValueBase<T> self,T endValue,float time)
            where T : IEquatable<T>
        {
            self.AddComponent(out TreeTween<T> treeTween, endValue, time).Run();
            return treeTween;
        }
    }
}
