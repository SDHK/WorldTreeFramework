using System.Collections.Generic;

namespace WorldTree
{

	//(T)System.Convert.ChangeType(Value, typeof(T));

	/// <summary>
	/// 设备类型
	/// </summary>
	public enum InputDeviceType : byte
	{
		/// <summary>
		/// 鼠标
		/// </summary>
		Mouse,
		/// <summary>
		/// 键盘
		/// </summary>
		Keyboard,
		/// <summary>
		/// 触摸屏
		/// </summary>
		Touch,
		/// <summary>
		/// 手柄
		/// </summary>
		Gamepad,
		/// <summary>
		/// 方向盘
		/// </summary>
		Wheel,
	}

	/// <summary>
	/// 输入类型
	/// </summary>
	public enum InputType : byte
	{
		/// <summary>
		/// 0~1区间型，如键盘按键、扳机、踏板
		/// </summary>
		Press,

		/// <summary>
		/// -1~1区间型，如方向盘、单轴摇杆
		/// </summary>
		Axis,

		/// <summary>
		/// XY轴区间型，如摇杆
		/// </summary>
		Axis2,

		/// <summary>
		/// XYZ轴区间型
		/// </summary>
		Axis3,

		/// <summary>
		/// 差值型，如鼠标滚轮、旋钮
		/// </summary>
		Delta,

		/// <summary>
		/// XY轴差值型，如鼠标移动
		/// </summary>
		Delta2,

		/// <summary>
		/// XYZ轴差值型
		/// </summary>
		Delta3,
	}

	/// <summary>
	/// 输入状态
	/// </summary>
	public enum InputState : byte
	{
		/// <summary>
		/// 无
		/// </summary>
		None = 0,
		/// <summary>
		/// 开始
		/// </summary>
		Start,
		/// <summary>
		/// 结束
		/// </summary>
		End,
		/// <summary>
		/// 活跃中
		/// </summary>
		Active,
	}


	/// <summary>
	/// 输入配置
	/// </summary>
	public struct InputConfig { }


	/// <summary>
	/// 输入管理器
	/// </summary>
	public class InputManager : Node
	{
		/// <summary>
		/// 输入设备集合
		/// </summary>
		public Dictionary<InputDeviceType, List<InputDevice>> InputDeviceDict;

	}






}
