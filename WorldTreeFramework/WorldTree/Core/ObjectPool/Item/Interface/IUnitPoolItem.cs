/******************************

 * Author: 闪电黑客

 * 日期: 2021/12/13 13:40:18

 * 最后日期: 2021/12/15 18:32:23

 * 最后修改: 闪电黑客

 * 描述:  
  
    泛型对象接口
    让IUnit类获得对象池管理的生命周期

******************************/

namespace WorldTree
{

    /// <summary>
    /// 单位池对象接口
    /// </summary>
    public interface IUnitPoolItem : IUnit
    {
        /// <summary>
        /// 产生此类的对象池：由对象池自动赋值，用于实例的自我回收
        /// </summary>
        IPool thisPool { get; set; }

        /// <summary>
        /// 回收标记
        /// </summary>
        bool IsRecycle { get; set; }

        /// <summary>
        /// 对象回收
        /// </summary>
        void Recycle();

    }

    public interface IUnitPoolItemEvent
    {
       

        /// <summary>
        /// 对象新建时
        /// </summary>
        void OnNew();

        /// <summary>
        /// 对象获取时
        /// </summary>
        void OnGet();

        /// <summary>
        /// 对象回收时
        /// </summary>
        void OnRecycle();
    }
}