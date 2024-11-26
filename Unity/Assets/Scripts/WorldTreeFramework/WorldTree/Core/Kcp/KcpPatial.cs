namespace ET
{

	public partial class Kcp
    {
		/// <summary>
		/// a?
		/// </summary>
		public const int ONE_M = 1024 * 1024;
        /// <summary>
        /// a?
        /// </summary>
        public const int INNER_MAX_WAIT_SIZE = 1024 * 1024;
        /// <summary>
        /// a?
        /// </summary>
        public const int OUTER_MAX_WAIT_SIZE = 1024 * 1024;

		/// <summary>
		/// 片段头部
		/// </summary>
		public struct SegmentHead
        {
			/// <summary>
			/// 是否是重传包?
			/// </summary>
			public uint Conv;

			/// <summary>
			/// 命令?
			/// </summary>
			public byte Cmd;
			/// <summary>
			/// 是否是重传包?
			/// </summary>
			public byte Frg;
			/// <summary>
			/// 窗口大小?
			/// </summary>
			public ushort Wnd;
			/// <summary>
			/// 时间戳?
			/// </summary>
			public uint Ts;
			/// <summary>
			/// 序号?
			/// </summary>
			public uint Sn;
			/// <summary>
			/// una?
			/// </summary>
			public uint Una;
			/// <summary>
			/// 数据长度?
			/// </summary>
			public uint Len;
        }
    }
    
    
}