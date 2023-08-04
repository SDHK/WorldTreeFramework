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
    public abstract class UnitPoolItem : Unit, IUnitPoolEventItem
    {
        public WorldTreeCore Core { get; set; }

        public bool IsFromPool { get; set; }

        public bool IsRecycle { get; set; }

        /// <summary>
        /// 回收对象
        /// </summary>
        public override void Dispose()
        {
            Core?.Recycle(this);
        }

        public virtual void OnGet()
        {
        }

        public virtual void OnNew()
        {
        }

        public virtual void OnRecycle()
        {
        }

    }
}
