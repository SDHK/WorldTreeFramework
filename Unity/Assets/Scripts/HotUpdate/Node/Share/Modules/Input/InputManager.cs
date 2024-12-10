namespace WorldTree
{
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
		/// 输入控制集合
		/// </summary>
		public InputControlGroup[] inputControlGroups;
	}

	/// <summary>
	/// 输入控制集合
	/// </summary>
	public class InputControlGroup : Node
	{
		/// <summary>
		/// 按键类型
		/// </summary>
		public byte KeyType;

		/// <summary>
		/// 输入控制集合
		/// </summary>
		public int[] InputValues;

		/// <summary>
		/// 输入状态集合
		/// </summary>
		public InputState[] InputStates;


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




	}


}
