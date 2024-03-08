/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/8 15:26

* 描述： 

*/

using System.Runtime.CompilerServices;

namespace WorldTree
{
    public static class MathQuaternionFloat
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(this QuaternionFloat a, QuaternionFloat b) => (float)((double)a.x * (double)b.x + (double)a.y * (double)b.y + (double)a.z * (double)b.z + (double)a.w * (double)b.w);



    }
}
