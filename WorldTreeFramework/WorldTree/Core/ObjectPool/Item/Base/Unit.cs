/****************************************

* 作者： 闪电黑客
* 日期： 2022/5/17 11:25

* 描述：单位类
* 用于自定义类的最基层
* 统一自定义类的释放功能和释放标记

*/

namespace WorldTree
{
    /// <summary>
    /// 单位基类
    /// </summary>
    public abstract class Unit : IUnit
    {
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

        public virtual void OnDispose() { }
        
    }

}
