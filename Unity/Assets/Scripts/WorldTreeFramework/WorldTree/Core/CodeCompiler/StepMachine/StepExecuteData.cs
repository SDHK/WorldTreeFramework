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