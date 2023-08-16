/****************************************

* 作者： 闪电黑客
* 日期： 2023/8/9 15:42

* 描述： 默认类型: 直接使用default会造成反射创建和产生GC，所以存起来使用

*/

namespace WorldTree
{
    /// <summary>
    /// 默认类型缓存，在泛型无法使用null时使用
    /// </summary>
    /// <remarks>直接使用default会造成反射创建和产生GC，所以存起来使用，会占用的内存大小与泛型类型有关</remarks>
    public static class DefaultType<T>
    {
        /// <summary>
        /// 默认值
        /// </summary>
        public static readonly T Default = default;
    }



}
