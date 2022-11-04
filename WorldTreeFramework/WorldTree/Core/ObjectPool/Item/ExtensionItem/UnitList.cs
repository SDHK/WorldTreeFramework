
/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/12 16:23

* 描述： 单位列表，这个列表可由对象池管理生成和回收
* 
* 其余和普通的List一样使用

*/

using System.Collections.Generic;

namespace WorldTree
{
    /// <summary>
    /// 单位列表：可由对象池管理回收
    /// </summary>
    public class UnitList<T> : List<T>, IUnitPoolEventItem
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
