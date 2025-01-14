/****************************************

* 作者：闪电黑客
* 日期：2024/12/21 18:09

* 描述：

*/
using System;

namespace WorldTree
{

	/// <summary>
	/// 输入设备信息
	/// </summary>
	public struct InputInfo : IEquatable<InputInfo>
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

		public override bool Equals(object obj)
		{
			if (obj is not InputInfo) return false;
			InputInfo other = (InputInfo)obj;
			return Equals(other);
		}

		public bool Equals(InputInfo other)
		=> InputDeviceType == other.InputDeviceType &&
			InputDeviceId == other.InputDeviceId &&
			InputType == other.InputType &&
			InputCode == other.InputCode;

		public override int GetHashCode() => HashCode.Combine(InputDeviceType, InputDeviceId, InputType, InputCode);

		public static bool operator ==(InputInfo left, InputInfo right) => left.Equals(right);

		public static bool operator !=(InputInfo left, InputInfo right) => !left.Equals(right);

	}
}