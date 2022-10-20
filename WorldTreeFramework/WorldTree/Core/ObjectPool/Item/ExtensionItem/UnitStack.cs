using System.Collections.Generic;

namespace WorldTree
{
    public class UnitStack<T> : Stack<T>, IUnitPoolItem, IUnitPoolItemEvent
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
