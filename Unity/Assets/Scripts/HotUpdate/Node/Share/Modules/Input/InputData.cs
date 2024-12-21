namespace WorldTree
{
	/// <summary>
	/// 输入数据
	/// </summary>
	public struct InputData
	{
		/// <summary>
		/// 设备类型
		/// </summary>
		public InputDeviceType InputDeviceType;

		/// <summary>
		/// 设备索引号
		/// </summary>
		public int InputDeviceId;

		/// <summary>
		/// 输入类型
		/// </summary>
		public InputType InputType;

		/// <summary>
		/// 输入状态
		/// </summary>
		public InputState InputState;

		/// <summary>
		/// 控件码
		/// </summary>
		public byte InputCode;

		/// <summary>
		/// X值 
		/// </summary>
		public int X;

		/// <summary>
		/// Y值
		/// </summary>
		public int Y;

		/// <summary>
		/// Z值
		/// </summary>
		public int Z;

		/// <summary>
		/// 时间戳（毫秒）
		/// </summary>
		public long TimeStamp;
	}


}
