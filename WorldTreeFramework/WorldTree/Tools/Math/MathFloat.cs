
/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/6 11:41

* 描述： 

*/


using System;

namespace WorldTree
{
    public static class MathFloat
    {
        public static volatile float FloatMinNormal = 1.1754944E-38f;
        public static volatile float FloatMinDenormal = float.Epsilon;
        public static bool IsFlushToZeroEnabled = (double)FloatMinDenormal == 0.0;

        public static readonly float Epsilon = IsFlushToZeroEnabled ? FloatMinNormal : FloatMinDenormal;

        /// <summary>
        ///   <para>The well-known 3.14159265358979... value (Read Only).</para>
        /// </summary>
        public const float PI = 3.1415927f;
        /// <summary>
        ///   <para>A representation of positive infinity (Read Only).</para>
        /// </summary>
        public const float Infinity = float.PositiveInfinity;
        /// <summary>
        ///   <para>A representation of negative infinity (Read Only).</para>
        /// </summary>
        public const float NegativeInfinity = float.NegativeInfinity;
        /// <summary>
        ///   <para>Degrees-to-radians conversion constant (Read Only).</para>
        /// </summary>
        public const float Deg2Rad = 0.017453292f;
        /// <summary>
        ///   <para>Radians-to-degrees conversion constant (Read Only).</para>
        /// </summary>
        public const float Rad2Deg = 57.29578f;

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
    }
}
