namespace WorldTree
{
	//(T)System.Convert.ChangeType(Value, typeof(T));

	//设备序号 + 设备类型 + 控件码 + 控件类型

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
}
