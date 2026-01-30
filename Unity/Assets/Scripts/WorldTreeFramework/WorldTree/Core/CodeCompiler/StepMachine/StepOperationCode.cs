namespace WorldTree
{

	/// <summary>
	/// 操作码掩码
	/// </summary>
	public static class StepOpCodeMask
	{
		/// <summary> 类别掩码（高4位） </summary>
		public const byte TYPE_MASK = 0xF0;
		/// <summary> 序号掩码（低4位） </summary>
		public const byte INDEX_MASK = 0x0F;
	}

	/// <summary>
	/// 步骤操作类型 
	/// </summary>
	public enum StepOperationType : byte
	{
		/// <summary> 无操作 </summary>
		None = 0,
		/// <summary> 算数运算 </summary>
		Math = 1 << 4,
		/// <summary> 比较运算 </summary>
		Compare = 2 << 4,
		/// <summary> 逻辑运算 </summary>
		Logic = 3 << 4,
		/// <summary> 位运算 </summary>
		Bit = 4 << 4,
		/// <summary> 栈操作 </summary>
		Stack = 5 << 4,
		/// <summary> 分支 </summary>
		Branch = 6 << 4,
		/// <summary> 循环 </summary>
		Loop = 7 << 4,
	}

	/// <summary> 
	/// 步骤操作码
	/// </summary>
	public enum StepOperationCode : byte
	{
		/// <summary> 无操作  </summary>
		None = 0,

		#region 运算

		#region 算术运算

		/// <summary> 加法 </summary>
		Add = StepOperationType.Math,
		/// <summary> 减法 </summary>
		Sub,
		/// <summary> 乘法 </summary>
		Mul,
		/// <summary> 除法 </summary>
		Div,
		/// <summary> 取模 </summary>
		Mod,

		#endregion

		#region 比较运算

		/// <summary> 等于 </summary>
		Eq = StepOperationType.Compare,
		/// <summary> 不等于 </summary>
		NotEq,
		/// <summary> 大于 </summary>
		Greater,
		/// <summary> 大于等于 </summary>
		GreaterEq,
		/// <summary> 小于 </summary>
		Less,
		/// <summary> 小于等于 </summary>
		LessEq,

		#endregion

		#region 逻辑运算

		/// <summary> 与 </summary>
		And = StepOperationType.Logic,
		/// <summary> 或 </summary>
		Or,
		/// <summary> 非(一元) </summary>
		Not,

		#endregion

		#region 位运算（可选）

		/// <summary> 按位与 </summary>
		BitAnd = StepOperationType.Bit,
		/// <summary> 按位或 </summary>
		BitOr,
		/// <summary> 按位异或 </summary>	
		BitXor,
		/// <summary> 按位取反(一元) </summary>
		BitNot,
		/// <summary> 左移(一元) </summary>
		BitShiftLeft,
		/// <summary> 右移(一元) </summary>
		BitShiftRight,

		#endregion

		#endregion

		#region 操作

		#region 栈操作

		/// <summary> 压入 </summary>
		Push = StepOperationType.Stack,
		/// <summary> 弹出 </summary>
		Pop,
		/// <summary> 压入变量值 </summary>
		PushVar,
		/// <summary> 弹出到变量 </summary>
		PopVar,
		/// <summary> 复制栈顶 </summary>
		Dup,

		#endregion

		#region 分支操作

		/// <summary> If </summary>
		IfPop = StepOperationType.Branch,
		/// <summary> Else </summary>
		Else,
		/// <summary> EndIf </summary>
		EndIf,
		/// <summary> 跳转标记 </summary>
		JumpLabel,
		/// <summary> 跳转 </summary>
		Jump,

		#endregion

		#region 循环操作

		///<summary> 循环开始检测</summary>
		LoopPop = StepOperationType.Loop,
		///<summary> 循环结束检测</summary>
		LoopEndPop,
		/// <summary> 循环开始 </summary>
		Loop,
		/// <summary> 循环结束 </summary>
		LoopEnd,
		/// <summary> Break </summary>
		Break,
		/// <summary> Continue </summary>
		Continue,

		#endregion


		#endregion
	}
}
