
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
    /// 浮点绑定器
    /// </summary>
    public class BinderFloat : Entity
    {
        /// <summary>
        /// 绑定获取
        /// </summary>
        public Func<float> GetValue;
        /// <summary>
        /// 绑定设置
        /// </summary>
        public Action<float> SetValue;

        /// <summary>
        /// 绑定数值的属性
        /// </summary>
        public float Value { get => GetValue(); set => SetValue(value); }
    }


}
