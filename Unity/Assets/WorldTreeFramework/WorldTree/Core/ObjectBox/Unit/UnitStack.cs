
/****************************************

* 作者： 闪电黑客
* 日期： 2022/11/10 10:12

* 描述： 单位栈，这个栈可由对象池管理生成和回收
* 
* 其余和普通的Stack一样使用

*/

using System.Collections.Generic;

namespace WorldTree
{
    public class UnitStack<T> : Stack<T>, IUnitPoolEventItem
    {
        public WorldTreeCore Core { get; set; }
        public long Type { get; set; }
        public bool IsFromPool { get; set; }
        public bool IsRecycle { get; set; }
        public bool IsDisposed { get; set; }


        public virtual void OnDispose()
        {
        }

        public virtual void OnGet()
        {
        }

        public virtual void OnNew()
        {
        }

        public virtual void OnRecycle()
        {
            Clear();
        }


        public void Dispose()
        {
            Core?.Recycle(this);
        }
    }
}
