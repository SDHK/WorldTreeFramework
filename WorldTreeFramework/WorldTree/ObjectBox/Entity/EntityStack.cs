
/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 10:55

* 描述： 实体泛型栈

*/

using System.Collections.Generic;

namespace WorldTree
{
    /// <summary>
    /// 实体泛型栈
    /// </summary>
    public class EntityStack<T> : Entity
    {
        public Stack<T> Value;
        public EntityStack() : base()
        {
            Value = new Stack<T>(); //初始化赋值
        }
        public override void OnDispose()
        {
            Value.Clear();
            base.OnDispose();
        }
    }
}
