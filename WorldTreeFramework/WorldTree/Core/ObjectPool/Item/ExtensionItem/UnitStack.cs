using System.Collections.Generic;

namespace WorldTree
{
    public class UnitStack<T> : Stack<T>, IUnitPoolItem, IUnitPoolItemEvent
    {
        public IPool thisPool { get; set; }
        public bool IsRecycle { get; set; }
        public bool IsDisposed { get; set; }


        /// <summary>
        /// 直接释放：释放后IsDisposed标记为true
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed) return;
            OnDispose();
            IsDisposed = true;
        }

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


        public void Recycle()
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
