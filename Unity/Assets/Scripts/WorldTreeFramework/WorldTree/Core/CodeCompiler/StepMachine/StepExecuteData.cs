using System;

namespace WorldTree
{
	/// <summary>
	/// 步骤执行数据
	/// </summary>
	public struct StepExecuteData
	{
		/// <summary>
		/// 处理器执行委托
		/// </summary>
		public Func<int, int, int> Execute;

		/// <summary>
		/// 数据地址
		/// </summary>
		public int Address;

		public StepExecuteData(Func<int, int, int> execute, int address)
		{
			Execute = execute;
			Address = address;
		}

		/// <summary>
		/// 执行步骤
		/// </summary>
		public int Run(int pointer) => Execute.Invoke(Address, pointer);
	}
}


//namespace WorldTree
//{
//	/// <summary>
//	/// 步骤执行数据
//	/// <para>纯数据结构，不绑定到特定实例，可序列化和复用</para>
//	/// </summary>
//	public struct StepExecuteData
//	{
//		/// <summary>
//		/// 操作码
//		/// </summary>
//		public StepOpCode OpCode;

//		/// <summary>
//		/// 数据地址
//		/// </summary>
//		public int DataAddress;

//		public StepExecuteData(StepOpCode opCode, int dataAddress = 0)
//		{
//			OpCode = opCode;
//			DataAddress = dataAddress;
//		}
//	}
//}
