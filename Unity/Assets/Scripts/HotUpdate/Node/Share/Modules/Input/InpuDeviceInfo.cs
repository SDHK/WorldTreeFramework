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
		public InpuDeviceInfo Device;

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
	public struct InpuDeviceInfo
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
}