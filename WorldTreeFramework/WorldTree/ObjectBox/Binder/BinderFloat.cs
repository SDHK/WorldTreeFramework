
/****************************************

* 作者： 闪电黑客
* 日期： 2023/2/15 15:08

* 描述： 浮点绑定器
* 
* 用于数值监听

*/

using System;

namespace WorldTree
{

    /// <summary>
    /// 值绑定器
    /// </summary>
    public class ValueBinder<T> : Node
    {
        /// <summary>
        /// 绑定对象
        /// </summary>
        public object bindObject;

        /// <summary>
        /// 绑定获取
        /// </summary>
        public Func<object, T> GetValue;

        /// <summary>
        /// 绑定设置
        /// </summary>
        public Action<object, T> SetValue;

        /// <summary>
        /// 绑定数值的属性
        /// </summary>
        public T Value { get { return GetValue(bindObject); } set { SetValue(bindObject, value); } }
    }
}
