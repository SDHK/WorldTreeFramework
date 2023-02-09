
/****************************************

* 作者： 闪电黑客
* 日期： 2023/1/18 19:23

* 描述： 单位HashSet，可由对象池管理生成和回收
* 
* 其余和普通的HashSet一样使用

*/

using System.Collections.Generic;

namespace WorldTree
{
    public class UnitHashSet<T>:HashSet<T>, IUnitPoolEventItem
    {
        public IPool thisPool { get; set; }
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
            if (thisPool != null)
            {
                if (!thisPool.IsDisposed)
                {
                    if (!IsRecycle)
                    {
                        thisPool.Recycle(this);
                    }
                }
            }
        }
    }
}
