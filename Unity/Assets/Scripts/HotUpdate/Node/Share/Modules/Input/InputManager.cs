namespace WorldTree
{


	/// <summary>
	/// 输入管理器
	/// </summary>
	public class InputManager : Node
	{
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


		//(T)System.Convert.ChangeType(Value, typeof(T));
	}


}
