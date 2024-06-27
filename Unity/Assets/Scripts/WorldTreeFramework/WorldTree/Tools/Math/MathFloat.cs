
/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/6 11:41

* 描述： 

*/


using System;

namespace WorldTree
{
	/// <summary>
	/// 浮点数数学类型
	/// </summary>
	public static class MathFloat
    {
		/// <summary>
		/// 浮点数的最小正常值。
		/// </summary>
		public static volatile float FloatMinNormal = 1.1754944E-38f;
		/// <summary>
		/// 浮点数的最小非正常值。
		/// </summary>
		public static volatile float FloatMinDenormal = float.Epsilon;
		/// <summary>
		/// 是否启用零值截断。
		/// </summary>
		public static bool IsFlushToZeroEnabled = (double)FloatMinDenormal == 0.0;

		/// <summary>
		/// 一个非常小的浮点数，用于比较浮点数是否接近零。
		/// </summary>
		public static readonly float Epsilon = IsFlushToZeroEnabled ? FloatMinNormal : FloatMinDenormal;

        /// <summary>
        ///   <para>The well-known 3.14159265358979... value (Read Only).</para>
        /// </summary>
        public const float PI = 3.1415927f;
        /// <summary>
        ///   <para>A representation of positive infinity (Read Only).</para>
        /// </summary>
        public const float INFINITY = float.PositiveInfinity;
        /// <summary>
        ///  <para>A representation of negative infinity (Read Only).</para>
        /// </summary>
        public const float NEGATIVE_INFINITY = float.NegativeInfinity;
        /// <summary>
        ///  <para>Degrees-to-radians conversion constant (Read Only).</para>
        /// </summary>
        public const float DEG_RAD = 0.017453292f;
        /// <summary>
        ///   <para>Radians-to-degrees conversion constant (Read Only).</para>
        /// </summary>
        public const float RAD_DEG = 57.29578f;

        /// <summary>
        ///   <para>Returns the sine of angle f.</para>
        /// </summary>
        /// <param name="f">The input angle, in radians.</param>
        /// <returns>
        ///   <para>The return value between -1 and +1.</para>
        /// </returns>
        public static float Sin(float f) => (float)Math.Sin((double)f);

        /// <summary>
        ///   <para>Returns the cosine of angle f.</para>
        /// </summary>
        /// <param name="f">The input angle, in radians.</param>
        /// <returns>
        ///   <para>The return value between -1 and 1.</para>
        /// </returns>
        public static float Cos(float f) => (float)Math.Cos((double)f);

        /// <summary>
        ///   <para>Returns the tangent of angle f in radians.</para>
        /// </summary>
        /// <param name="f"></param>
        public static float Tan(float f) => (float)Math.Tan((double)f);

        /// <summary>
        ///   <para>Returns the arc-sine of f - the angle in radians whose sine is f.</para>
        /// </summary>
        /// <param name="f"></param>
        public static float Asin(float f) => (float)Math.Asin((double)f);

        /// <summary>
        ///   <para>Returns the arc-cosine of f - the angle in radians whose cosine is f.</para>
        /// </summary>
        /// <param name="f"></param>
        public static float Acos(float f) => (float)Math.Acos((double)f);

        /// <summary>
        ///   <para>Returns the arc-tangent of f - the angle in radians whose tangent is f.</para>
        /// </summary>
        /// <param name="f"></param>
        public static float Atan(float f) => (float)Math.Atan((double)f);

        /// <summary>
        ///   <para>Returns the angle in radians whose Tan is y/x.</para>
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        public static float Atan2(float y, float x) => (float)Math.Atan2((double)y, (double)x);

        /// <summary>
        ///   <para>Returns square root of f.</para>
        /// </summary>
        /// <param name="f"></param>
        public static float Sqrt(float f) => (float)Math.Sqrt((double)f);

        /// <summary>
        ///   <para>Returns the absolute value of f.</para>
        /// </summary>
        /// <param name="f"></param>
        public static float Abs(float f) => Math.Abs(f);

        /// <summary>
        ///   <para>Returns the absolute value of value.</para>
        /// </summary>
        /// <param name="value"></param>
        public static int Abs(int value) => Math.Abs(value);

        /// <summary>
        ///   <para>Returns the smallest of two or more values.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="values"></param>
        public static float Min(float a, float b) => (double)a < (double)b ? a : b;

        /// <summary>
        ///   <para>Returns the smallest of two or more values.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="values"></param>
        public static float Min(params float[] values)
        {
            int length = values.Length;
            if (length == 0)
                return 0.0f;
            float num = values[0];
            for (int index = 1; index < length; ++index)
            {
                if ((double)values[index] < (double)num)
                    num = values[index];
            }
            return num;
        }

        /// <summary>
        ///   <para>Returns the smallest of two or more values.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="values"></param>
        public static int Min(int a, int b) => a < b ? a : b;

        /// <summary>
        ///   <para>Returns the smallest of two or more values.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="values"></param>
        public static int Min(params int[] values)
        {
            int length = values.Length;
            if (length == 0)
                return 0;
            int num = values[0];
            for (int index = 1; index < length; ++index)
            {
                if (values[index] < num)
                    num = values[index];
            }
            return num;
        }

        /// <summary>
        ///   <para>Returns largest of two or more values.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="values"></param>
        public static float Max(float a, float b) => (double)a > (double)b ? a : b;

        /// <summary>
        ///   <para>Returns largest of two or more values.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="values"></param>
        public static float Max(params float[] values)
        {
            int length = values.Length;
            if (length == 0)
                return 0.0f;
            float num = values[0];
            for (int index = 1; index < length; ++index)
            {
                if ((double)values[index] > (double)num)
                    num = values[index];
            }
            return num;
        }

        /// <summary>
        ///   <para>Returns the largest of two or more values.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="values"></param>
        public static int Max(int a, int b) => a > b ? a : b;

        /// <summary>
        ///   <para>Returns the largest of two or more values.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="values"></param>
        public static int Max(params int[] values)
        {
            int length = values.Length;
            if (length == 0)
                return 0;
            int num = values[0];
            for (int index = 1; index < length; ++index)
            {
                if (values[index] > num)
                    num = values[index];
            }
            return num;
        }

        /// <summary>
        /// 夹住0到1之间的值并返回值。
        /// </summary>
        /// <param name="value"></param>
        public static float Clamp01(float value)
        {
            if ((double)value < 0.0)
                return 0.0f;
            return (double)value > 1.0 ? 1f : value;
        }

        /// <summary>
        /// 计算两个给定角度之间的最小差。
        /// </summary>
        public static float DeltaAngle(float current, float target)
        {
            float num = MathFloat.Repeat(target - current, 360f);
            if ((double)num > 180.0)
                num -= 360f;
            return num;
        }

        /// <summary>
        /// 返回小于或等于f的最大整数。
        /// </summary>
        public static float Floor(float f) => (float)Math.Floor((double)f);

        /// <summary>
        /// 循环值t，使其不大于length且不小于0。
        /// </summary>
        public static float Repeat(float t, float length) => Math.Clamp(t - MathFloat.Floor(t / length) * length, 0.0f, length);


        /// <summary>
        /// 在a和b之间用t线性插值。(内部限制t在0~1之间)
        /// </summary>
        /// <param name="a">起始值</param>
        /// <param name="b">最终值</param>
        /// <param name="t">两个浮点数之间的插值值。</param>
        public static float Lerp(float a, float b, float t) => a + (b - a) * MathFloat.Clamp01(t);

        /// <summary>
        /// 在a和b之间用t线性插值，对t没有限制。
        /// </summary>
        /// <param name="a">起始值</param>
        /// <param name="b">最终值</param>
        /// <param name="t">两个浮点数之间的插值值。</param>
        public static float LerpUnclamped(float a, float b, float t) => a + (b - a) * t;

        /// <summary>
        /// 与Lerp相同，但确保值在绕360度旋转时正确插入。
        /// </summary>
        public static float LerpAngle(float a, float b, float t)
        {
            float num = MathFloat.Repeat(b - a, 360f);
            if ((double)num > 180.0)
                num -= 360f;
            return a + num * MathFloat.Clamp01(t);
        }

        /// <summary>
        /// 向目标移动当前值。
        /// </summary>
        /// <param name="current">当前值</param>
        /// <param name="target">目标值</param>
        /// <param name="maxDelta">增量</param>
        public static float MoveTowards(float current, float target, float maxDelta) => (double)Math.Abs(target - current) <= (double)maxDelta ? target : current + Math.Sign(target - current) * maxDelta;


        /// <summary>
        /// 与MoveTowards相同，但要确保值在360度左右旋转时正确插入。
        /// </summary>
        /// <param name="current">当前值</param>
        /// <param name="target">目标值</param>
        /// <param name="maxDelta">增量</param>
        public static float MoveTowardsAngle(float current, float target, float maxDelta)
        {
            float num = MathFloat.DeltaAngle(current, target);
            if (-(double)maxDelta < (double)num && (double)num < (double)maxDelta)
                return target;
            target = current + num;
            return MoveTowards(current, target, maxDelta);
        }

        /// <summary>
        /// 在最小值和最大值之间进行插值，在极限处进行平滑。
        /// </summary>
        public static float SmoothStep(float from, float to, float t)
        {
            t = MathFloat.Clamp01(t);
            t = (float)(-2.0 * (double)t * (double)t * (double)t + 3.0 * (double)t * (double)t);
            return (float)((double)to * (double)t + (double)from * (1.0 - (double)t));
        }
    }
}
