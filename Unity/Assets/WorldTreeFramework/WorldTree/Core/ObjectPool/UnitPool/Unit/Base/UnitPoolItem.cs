/****************************************

* 作者： 闪电黑客
* 日期： 2022/5/18 15:07

* 描述： 池单位对象，抽象基类
* 
* 抽象基类将提供一个回收实例的方法，
* 并对其是否可以回收进行判断。

*/

namespace WorldTree
{


    /// <summary>
    /// 池单位抽象基类：提供回收方法
    /// </summary>
    public abstract class UnitPoolItem : IUnitPoolEventItem
    {
        public bool IsDisposed { get; set; }

        public WorldTreeCore Core { get; set; }

        public bool IsRecycle { get; set; }



        public virtual void OnGet()
        {
        }

        public virtual void OnNew()
        {
        }

        public virtual void OnRecycle()
        {
        }


        /// <summary>
        /// 回收对象
        /// </summary>
        public virtual void Dispose()
        {
            if (IsDisposed) return;
            OnDispose();
            IsDisposed = true;
        }

        public virtual void OnDispose() { }

    }
}
