namespace WorldTree
{
	/// <summary>
	/// 输入控制集合
	/// </summary>
	public class InputDevice : Node
	{
		/// <summary>
		/// 设备码
		/// </summary>
		public int DeviceId;

		/// <summary>
		/// 设备类型
		/// </summary>
		public InputDeviceType InputDeviceType;


		/// <summary>
		/// 输入数据集合，下标是按键码
		/// </summary>
		public InputData[] InputDatas;

	}


}
