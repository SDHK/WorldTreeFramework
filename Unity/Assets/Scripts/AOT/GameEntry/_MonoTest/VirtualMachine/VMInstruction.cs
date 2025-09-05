using System.Collections.Generic;

namespace VM
{

	#region 虚拟机指令系统
	/// <summary>
	/// 虚拟机指令类型
	/// </summary>
	public enum VMInstruction
	{
		// === 数据操作指令 ===
		LOAD_CONST,     // 加载常量到栈顶
		LOAD_VAR,       // 加载变量到栈顶
		STORE_VAR,      // 将栈顶值存储到变量

		// === 算术运算指令 ===
		ADD,            // 栈顶两个值相加
		SUB,            // 栈顶两个值相减
		MUL,            // 栈顶两个值相乘
		DIV,            // 栈顶两个值相除
		MOD,            // 栈顶两个值取模
		POW,            // 栈顶两个值幂运算
		NEG,            // 栈顶值取负

		// === 比较运算指令 ===
		EQ,             // 相等比较
		NE,             // 不相等比较
		LT,             // 小于比较
		LE,             // 小于等于比较
		GT,             // 大于比较
		GE,             // 大于等于比较

		// === 逻辑运算指令 ===
		AND,            // 逻辑与
		OR,             // 逻辑或
		NOT,            // 逻辑非

		// === 控制流指令 ===
		JMP,            // 无条件跳转
		JMP_IF_FALSE,   // 条件跳转（假时跳转）
		JMP_IF_TRUE,    // 条件跳转（真时跳转）

		// === 函数相关指令 ===
		CALL_FUNC,      // 调用函数
		DEFINE_FUNC,    // 定义函数（带参数类型和返回类型）
		RETURN,         // 函数返回（带返回值）
		RETURN_VOID,    // 函数返回（无返回值）

		// === 栈操作指令 ===
		POP,            // 弹出栈顶值
		DUP,            // 复制栈顶值

		// === 特殊指令 ===
		NOP,            // 空操作
		HALT,           // 停机指令
	}



	/// <summary>
	/// 虚拟机指令结构
	/// </summary>
	public class VMInstructionData
	{
		public VMInstruction OpCode { get; set; }
		public object Operand { get; set; }
		public int LineNumber { get; set; }

		public VMInstructionData(VMInstruction opCode, object operand = null, int lineNumber = 0)
		{
			OpCode = opCode;
			Operand = operand;
			LineNumber = lineNumber;
		}

		public override string ToString()
		{
			return Operand != null ? $"{OpCode} {Operand}" : OpCode.ToString();
		}
	}

	#endregion

	#region 虚拟机指令生成器

	/// <summary>
	/// 虚拟机指令生成器
	/// </summary>
	public class VMCodeGenerator
	{
		private List<VMInstructionData> _instructions = new List<VMInstructionData>();
		private Dictionary<int, int> _labels = new Dictionary<int, int>();
		private List<int> _pendingLabels = new List<int>();
		private int _nextLabelId = 0;
		private int _currentLine = 0;

		/// <summary>
		/// 生成指令
		/// </summary>
		public void Emit(VMInstruction opCode, object operand = null)
		{
			_instructions.Add(new VMInstructionData(opCode, operand, _currentLine));
		}

		/// <summary>
		/// 分配标签
		/// </summary>
		public int AllocateLabel()
		{
			return _nextLabelId++;
		}

		/// <summary>
		/// 设置标签位置
		/// </summary>
		public void SetLabel(int labelId)
		{
			_labels[labelId] = _instructions.Count;
		}

		/// <summary>
		/// 获取生成的指令列表
		/// </summary>
		public List<VMInstructionData> GetInstructions()
		{
			// 解析标签引用
			for (int i = 0; i < _instructions.Count; i++)
			{
				var instruction = _instructions[i];
				if (instruction.Operand is int labelId && _labels.ContainsKey(labelId))
				{
					instruction.Operand = _labels[labelId];
				}
			}

			return new List<VMInstructionData>(_instructions);
		}

		/// <summary>
		/// 清空生成器
		/// </summary>
		public void Clear()
		{
			_instructions.Clear();
			_labels.Clear();
			_pendingLabels.Clear();
			_nextLabelId = 0;
			_currentLine = 0;
		}

		/// <summary>
		/// 设置当前行号
		/// </summary>
		public void SetCurrentLine(int lineNumber)
		{
			_currentLine = lineNumber;
		}

		/// <summary>
		/// 获取当前指令地址
		/// </summary>
		public int CurrentAddress => _instructions.Count;
	}

	#endregion

}
