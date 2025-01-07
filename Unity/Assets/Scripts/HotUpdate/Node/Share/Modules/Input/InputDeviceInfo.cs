/****************************************

* 作者：闪电黑客
* 日期：2024/12/21 18:09

* 描述：

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// 输入数据
	/// </summary>
	public struct InputData
	{
		/// <summary>
		/// 输入设备信息
		/// </summary>
		public InputDeviceInfo Device;

		/// <summary>
		/// 输入状态信息
		/// </summary>
		public InputInfo Info;

		/// <summary>
		/// 时间戳（毫秒）
		/// </summary>
		public DateTime TimeStamp;
	}

	/// <summary>
	/// 输入设备信息
	/// </summary>
	public struct InputDeviceInfo
	{
		/// <summary>
		/// 设备类型
		/// </summary>
		public InputDeviceType InputDeviceType;

		/// <summary>
		/// 设备索引号
		/// </summary>
		public byte InputDeviceId;

		/// <summary>
		/// 控件类型
		/// </summary>
		public InputType InputType;

		/// <summary>
		/// 控件码
		/// </summary>
		public byte InputCode;
	}

	/// <summary>
	/// 输入信息
	/// </summary>
	public struct InputInfo : IEquatable<InputInfo>
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
			if (obj is not InputInfo) return false;
			InputInfo other = (InputInfo)obj;
			return Equals(other);
		}

		public bool Equals(InputInfo other)
		{
			return InputState == other.InputState && X == other.X && Y == other.Y && Z == other.Z;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(InputState, X, Y, Z);
		}

		public static bool operator ==(InputInfo left, InputInfo right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(InputInfo left, InputInfo right)
		{
			return !(left == right);
		}
	}



	/// <summary>
	/// 输入驱动信息
	/// </summary>
	public struct InputDriverInfo : IEquatable<InputDriverInfo>
	{
		/// <summary>
		/// 控件类型
		/// </summary>
		public InputType InputType;

		/// <summary>
		/// 是否输入中
		/// </summary>
		public bool IsInput;

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

		public InputDriverInfo(InputType inputType, bool isInput, int x)
		{
			InputType = inputType;
			IsInput = isInput;
			X = x;
			Y = 0;
			Z = 0;
		}

		public InputDriverInfo(InputType inputType, bool isInput, int x, int y)
		{
			InputType = inputType;
			IsInput = isInput;
			X = x;
			Y = y;
			Z = 0;
		}

		public InputDriverInfo(InputType inputType, bool isInput, int x, int y, int z)
		{
			InputType = inputType;
			IsInput = isInput;
			X = x;
			Y = y;
			Z = z;
		}

		public override bool Equals(object obj)
		{
			if (obj is not InputDriverInfo) return false;
			InputDriverInfo other = (InputDriverInfo)obj;
			return Equals(other);
		}

		public bool Equals(InputDriverInfo other) => IsInput == other.IsInput && X == other.X && Y == other.Y && Z == other.Z;

		public override int GetHashCode() => HashCode.Combine(IsInput, X, Y, Z);

		public static bool operator ==(InputDriverInfo left, InputDriverInfo right) => left.Equals(right);

		public static bool operator !=(InputDriverInfo left, InputDriverInfo right) => !(left == right);
	}


}