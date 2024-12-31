/****************************************

* 作者： 闪电黑客
* 日期： 2024/12/21 18:09

* 描述：

*/

namespace WorldTree
{
	/// <summary>
	/// 输入设备
	/// </summary>
	public class InputDevice : Node, ChildOf<InputDeviceManager>
	, AsAwake
	{
		/// <summary>
		/// 设备类型
		/// </summary>
		public InputDeviceType InputDeviceType;

		/// <summary>
		/// 设备码
		/// </summary>
		public byte DeviceId;

		/// <summary>
		/// 输入数据集合，下标是按键码
		/// </summary>
		public InpuDeviceInfo[] InputDatas;
	}
}