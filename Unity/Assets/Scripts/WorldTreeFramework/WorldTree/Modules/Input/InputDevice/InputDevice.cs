/****************************************

* 作者：闪电黑客
* 日期：2024/12/21 18:09

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 输入设备
	/// </summary>
	public partial class InputDevice : Node, ChildOf<InputDeviceManager>
		, AsRule<Awake<InputDeviceType, byte>>
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
		public InputData[] InputDatas;

		[NodeRule(nameof(AwakeRule<InputDevice, InputDeviceType, byte>))]
		private static void OnAwakeRule(InputDevice self, InputDeviceType deviceType, byte deviceId)
		{
			self.InputDeviceType = deviceType;
			self.DeviceId = deviceId;
			self.InputDatas = new InputData[256];
		}

		[NodeRule(nameof(AddRule<InputDevice>))]
		private static void OnAddRule(InputDevice self)
		{
			self.InputDatas = new InputData[256];
		}

		/// <summary>
		/// 设置输入数据
		/// </summary>
		public void SetData(byte inputCode, InputData inputData)
		{
			this.InputDatas[inputCode] = inputData;
		}

	}
}
