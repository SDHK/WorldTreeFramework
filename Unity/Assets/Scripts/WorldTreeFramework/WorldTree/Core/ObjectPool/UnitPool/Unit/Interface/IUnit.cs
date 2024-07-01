
/****************************************

* 作者： 闪电黑客
* 日期： 2022/5/17 11:19

* 描述： 给实现接口的类一个统一的销毁标记，和方法

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 单位接口
    /// </summary>
    public interface IUnit : IDisposable
    {
        /// <summary>
        /// 类型码
        /// </summary>
        public long Type { get; set; }

        /// <summary>
        /// 释放标记
        /// </summary>
        bool IsDisposed { get; set; }

        /// <summary>
        /// 对象释放时的处理
        /// </summary>
        void OnDispose();
    }
}
