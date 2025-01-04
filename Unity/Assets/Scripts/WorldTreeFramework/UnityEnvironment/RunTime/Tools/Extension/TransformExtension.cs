/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using UnityEngine;

namespace WorldTree
{
    /// <summary>
    /// Transform组件扩展方法
    /// </summary>
    public static class TransformExtension
    {
        /// <summary>
        /// 将位置旋转和大小变为默认值
        /// </summary>
        public static void Default(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
    }
}
