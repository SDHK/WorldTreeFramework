/****************************************

* 作者： 闪电黑客
* 日期： 2024/12/31 11:37

* 描述： 

*/
namespace WorldTree
{
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
		public UnitList<InputDriverInfo[]> InputInfosList;

	}
}