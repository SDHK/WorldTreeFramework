/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 10:58

* 描述： 实体泛型HashSet

*/

using System.Collections.Generic;

namespace WorldTree
{
    /// <summary>
    /// 实体泛型HashSet
    /// </summary>
    public class EntityHashSet<T> : Node
    {
        public HashSet<T> Value;
        public EntityHashSet() : base()
        {
            Value = new HashSet<T>(); //初始化赋值
        }
        public override void OnDispose()
        {
            Value.Clear();
            base.OnDispose();
        }
    }
}
