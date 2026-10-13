namespace WorldTree
{
	/// <summary>
	/// 步骤汇编数据 
	/// </summary>
	public struct StepAssemblyData
	{
		/// <summary> 运算操作码 </summary>
		public StepOpCode OpCode;
		/// <summary> 字面量 </summary>
		public VarValue Literal;

		public StepAssemblyData(StepOpCode opCode, VarValue literal = default)
		{
			OpCode = opCode;
			Literal = literal;
		}
	}
}
