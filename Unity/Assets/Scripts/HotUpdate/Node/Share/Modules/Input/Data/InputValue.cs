/****************************************

* 作者：闪电黑客
* 日期：2024/12/21 18:09

* 描述：

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// 输入数值
	/// </summary>
	public struct InputValue : IEquatable<InputValue>
	{
		/// <summary>
		/// 输入状态
		/// </summary>
		public InputState InputState;

		/// <summary>
		/// X值
		/// </summary>
		public int X;

		/// <summary>
		/// Y值
		/// </summary>
		public int Y;

		/// <summary>
		/// Z值
		/// </summary>
		public int Z;

		public override bool Equals(object obj)
		{
			if (obj is not InputValue) return false;
			InputValue other = (InputValue)obj;
			return Equals(other);
		}

		public bool Equals(InputValue other)
		{
			return InputState == other.InputState && X == other.X && Y == other.Y && Z == other.Z;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(InputState, X, Y, Z);
		}

		public static bool operator ==(InputValue left, InputValue right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(InputValue left, InputValue right)
		{
			return !(left == right);
		}
	}


}