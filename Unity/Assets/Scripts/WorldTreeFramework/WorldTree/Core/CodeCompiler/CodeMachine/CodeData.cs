using System;

namespace WorldTree
{
	/// <summary>
	/// 指令数据
	/// </summary>
	public struct CodeData
	{
		/// <summary>
		/// 解析器
		/// </summary>
		public Func<int, int, int> Parser;

		/// <summary>
		/// 地址 
		/// </summary>
		public int Address;

		/// <summary>
		/// 执行指令 
		/// </summary>
		public int Execute(int pointer) => Parser.Invoke(Address, pointer);
	}
}
