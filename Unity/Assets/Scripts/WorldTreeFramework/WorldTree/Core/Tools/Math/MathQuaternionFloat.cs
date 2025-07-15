/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System.Runtime.CompilerServices;

namespace WorldTree
{
	/// <summary>
	/// 浮点四元数工具类
	/// </summary>
	public static class MathQuaternionFloat
    {
		/// <summary>
		/// 点乘
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(this QuaternionFloat a, QuaternionFloat b) => (float)((double)a.X * (double)b.X + (double)a.Y * (double)b.Y + (double)a.Z * (double)b.Z + (double)a.W * (double)b.W);
    }
}
