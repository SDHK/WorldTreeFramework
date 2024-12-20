namespace WorldTree
{


	/// <summary>
	/// 设备类型
	/// </summary>
	public enum InputEquipType : byte
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
		/// 一维数值
		/// </summary>
		One,
		/// <summary>
		/// 二维数值
		/// </summary>
		Two,
		/// <summary>
		/// 三维数值
		/// </summary>
		Three
	}

	/// <summary>
	/// 按键状态
	/// </summary>
	public enum InputState
	{
		/// <summary>
		/// 无
		/// </summary>
		None = 0,
		/// <summary>
		/// 按下
		/// </summary>
		Down,
		/// <summary>
		/// 抬起
		/// </summary>
		Up,
		/// <summary>
		/// 按住
		/// </summary>
		Hold,
	}


	/// <summary>
	/// 输入管理器
	/// </summary>
	public class InputManager : Node
	{
		/// <summary>
		/// 输入队列
		/// </summary>
		public UnitQueue<InputState> inputQueue;


		/// <summary>
		/// 输入控制集合，下标是设备类型
		/// </summary>
		public InputControlGroup[] inputControlGroups;
	}

	/// <summary>
	/// 输入控制集合
	/// </summary>
	public class InputControlGroup : Node
	{
		/// <summary>
		/// 设备类型
		/// </summary>
		public InputEquipType InputType;
		/// <summary>
		/// 设备码
		/// </summary>
		public int InputId;

		/// <summary>
		/// 输入数据集合，下标是按键码
		/// </summary>
		public InputData[] inputDatas;

		//(T)System.Convert.ChangeType(Value, typeof(T));
	}


	/// <summary>
	/// 输入数据
	/// </summary>
	public struct InputData
	{
		/// <summary>
		/// 输入状态
		/// </summary>
		public InputState InputState;

		/// <summary>
		/// 按键类型
		/// </summary>
		public InputType InputType;

		/// <summary>
		/// 按键码
		/// </summary>
		public byte KeyCode;

		/// <summary>
		/// 主输入值 - 使用int存储，可以根据需要转换为float
		/// 对于摇杆，可以存储-100到100的值表示百分比
		/// 对于鼠标，可以直接存储像素位置或移动距离
		/// </summary>
		public int Value { get; private set; }

		/// <summary>
		/// 辅助输入值 - 用于需要两个维度的输入（如摇杆的Y轴）
		/// </summary>
		public int Value2 { get; private set; }

		/// <summary>
		/// 时间戳（毫秒）
		/// </summary>
		public long Timestamp;
	}


	/// <summary>
	/// 输入配置
	/// </summary>
	public struct InputConfig
	{


	}


}
