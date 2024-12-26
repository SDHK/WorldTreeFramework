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

	/// <summary>
	/// 输入设备驱动器
	/// </summary>
	public abstract class InputDriver : Node, ComponentOf<InputDeviceManager>
		, AsAwake<InputDeviceManager>
	{
		/// <summary>
		/// 输入设备管理器
		/// </summary>
		[Protected] public InputDeviceManager inputManager;

		/// <summary>
		/// 设备类型
		/// </summary>
		public InputDeviceType DeviceType;

		/// <summary>
		/// 设备是否存在
		/// </summary>
		public bool[] IsExists;

		/// <summary>
		/// 输入控件类型
		/// </summary>
		public InputType[] InputTypes;

		/// <summary>
		/// 输入信息缓存
		/// </summary>
		public UnitList<InputInfo[]> InputInfosList;
	}

	//需要事件Rule，通过配置，注册到对应设备进行监听

	/// <summary>
	/// 输入控件:壳
	/// </summary>
	public class InputControlBase
	{
		/// <summary>
		/// 控件码
		/// </summary>
		public byte InputCode;

		/// <summary>
		/// 输入状态
		/// </summary>
		public InputState InputState;
	}

	/// <summary>
	/// 输入控件:按压控件
	/// </summary>
	public class InputControlPress : InputControlBase
	{
	}
}