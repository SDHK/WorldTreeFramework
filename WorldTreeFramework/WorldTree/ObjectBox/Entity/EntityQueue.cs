
/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 10:51

* 描述： 实体队列

*/

using System.Collections.Generic;

namespace WorldTree
{
    /// <summary>
    /// 实体泛型队列
    /// </summary>
    public class EntityQueue<T> : Node
    {
        public Queue<T> Value;
        public EntityQueue() : base()
        {
            Value = new Queue<T>(); //初始化赋值
        }
        public override void OnDispose()
        {
            Value.Clear();
            base.OnDispose();
        }
    }

   


}
