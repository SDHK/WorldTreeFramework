
/****************************************

* 作者： 闪电黑客
* 日期： 2023/8/16 21:24

* 描述： Null类型缓存：需要使用null参数推断泛型时使用

*/

namespace WorldTree
{
    /// <summary>
    /// Null类型缓存，需要使用null参数推断泛型时使用
    /// </summary>
    /// <remarks>直接使用Null会造成转换消耗，所以存起来使用，会占用一个指针大小的内存</remarks>
    public static class NullType<T>
        where T : class
    {
        /// <summary>
        /// 默认值
        /// </summary>
        public static readonly T Null = null;
    }
}
