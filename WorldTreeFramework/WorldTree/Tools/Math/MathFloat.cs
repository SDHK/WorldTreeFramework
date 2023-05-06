
/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/6 11:41

* 描述： 

*/


namespace WorldTree
{
    public static class MathFloat
    {
        public static volatile float FloatMinNormal = 1.1754944E-38f;
        public static volatile float FloatMinDenormal = float.Epsilon;
        public static bool IsFlushToZeroEnabled = (double)FloatMinDenormal == 0.0;

        public static readonly float Epsilon = IsFlushToZeroEnabled ? FloatMinNormal : FloatMinDenormal;


    }
}
