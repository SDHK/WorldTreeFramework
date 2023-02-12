
/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/9 14:34

* 描述： 异步任务接口

*/

using System;
using System.Runtime.CompilerServices;

namespace WorldTree
{
    /// <summary>
    /// 异步任务接口
    /// </summary>
    public interface IAsyncTask : ICriticalNotifyCompletion
    {
        /// <summary>
        /// 是否完成
        /// </summary>
        bool IsCompleted { get; set; }
        /// <summary>
        /// 获取结果
        /// </summary>
        void GetResult();

        /// <summary>
        /// 设置结果
        /// </summary>
        //void SetResult();
        public Action SetResult { get; set; }

        /// <summary>
        /// 设置异常
        /// </summary>
        /// <param name="exception"></param>
        void SetException(Exception exception);
    }

    /// <summary>
    /// 泛型异步任务接口
    /// </summary>
    public interface IAsyncTask<T> : ICriticalNotifyCompletion
    {
        /// <summary>
        /// 是否完成
        /// </summary>
        bool IsCompleted { get; set; }
        /// <summary>
        /// 获取结果
        /// </summary>
        T GetResult();
        /// <summary>
        /// 设置结果
        /// </summary>
        //void SetResult(T result);

        public Action<T> SetResult { get; set; }
        /// <summary>
        /// 设置异常
        /// </summary>
        void SetException(Exception exception);
    }
}
