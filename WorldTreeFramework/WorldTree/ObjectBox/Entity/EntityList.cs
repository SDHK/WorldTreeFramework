
/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 10:53

* 描述： 实体泛型列表

*/

using System.Collections.Generic;

namespace WorldTree
{
    /// <summary>
    /// 实体泛型列表
    /// </summary>
    public class EntityList<T> : Node
    {
        public List<T> Value;
        public EntityList() : base()
        {
            Value = new List<T>(); //初始化赋值
        }
        public override void OnDispose()
        {
            Value.Clear();
            base.OnDispose();
        }
    }


}
