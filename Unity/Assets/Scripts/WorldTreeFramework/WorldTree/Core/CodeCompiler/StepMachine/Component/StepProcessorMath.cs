namespace WorldTree
{
	/// <summary>
	/// 步骤处理器：数学运算
	/// </summary>
	public class StepProcessorMath : StepProcessor
	{
		/// <summary>
		/// 运算操作列表
		/// </summary>
		public UnitList<MathOperation> operationList;

		/// <summary>
		/// 执行数学运算步骤
		/// </summary>
		private int ExecuteMath(int address, int pointer)
		{
			MathOperation op = operationList[address];
			// 根据运算类型执行
			switch (op)
			{
				case MathOperation.Not:
				case MathOperation.BitNot: ExecuteUnaryOp(op, pointer); break;
				default: ExecuteBinaryOp(op, pointer); break;
			}
			return pointer + 1;
		}

		/// <summary>
		/// 执行二元运算
		/// </summary>
		private void ExecuteBinaryOp(MathOperation op, int pointer)
		{
			// 注意：栈顶是右操作数
			VarValue right = Machine.Pop();
			VarValue left = Machine.Pop();
			VarValue result = new VarValue();
			switch (op)
			{
				case MathOperation.Add: result = left + right; break;
				case MathOperation.Sub: result = left - right; break;
				case MathOperation.Mul: result = left * right; break;
				case MathOperation.Div:
					if (right == 0) this.LogError($"除零错误：{left} / {right}，位置：步骤{pointer}");
					result = left / right; break;
				case MathOperation.Mod: result = left % right; break;
				case MathOperation.Eq: result = left == right; break;
				case MathOperation.NotEq: result = left != right; break;
				case MathOperation.Greater: result = left > right; break;
				case MathOperation.GreaterEq: result = left >= right; break;
				case MathOperation.Less: result = left < right; break;
				case MathOperation.LessEq: result = left <= right; break;
				case MathOperation.And: result = left.ToBool() && right.ToBool(); break;
				case MathOperation.Or: result = left.ToBool() || right.ToBool(); break;
				case MathOperation.BitAnd: result = left & right; break;
				case MathOperation.BitOr: result = left | right; break;
				case MathOperation.BitXor: result = left ^ right; break;
			}
			Machine.Push(result);
		}

		/// <summary>
		/// 执行一元运算
		/// </summary>
		private void ExecuteUnaryOp(MathOperation op, int pointer)
		{
			VarValue operand = Machine.Pop();
			VarValue result = new VarValue();
			switch (op)
			{
				case MathOperation.Not: result = !operand.ToBool(); break;
				case MathOperation.BitNot: result = ~operand; break;
			}
			Machine.Push(result);
		}

		/// <summary>
		/// 添加数学运算步骤
		/// </summary>
		public void AddMathOp(MathOperation op)
		{
			operationList.Add(op);
			AddStep(new StepData(ExecuteMath, operationList.Count - 1));
		}
	}

	public static class StepProcessorMathRule
	{
		class Add : AddRule<StepProcessorMath>
		{
			protected override void Execute(StepProcessorMath self)
			{
				self.Core.PoolGetUnit(out self.operationList);
			}
		}

		class Remove : RemoveRule<StepProcessorMath>
		{
			protected override void Execute(StepProcessorMath self)
			{
				self.operationList.Dispose();
				self.operationList = null;
			}
		}
	}

	/// <summary> 
	/// 数学运算类型  
	/// </summary>
	public enum MathOperation : byte
	{
		// 算术运算
		/// <summary> 加法 </summary>
		Add,
		/// <summary> 减法 </summary>
		Sub,
		/// <summary> 乘法 </summary>
		Mul,
		/// <summary> 除法 </summary>
		Div,
		/// <summary> 取模 </summary>
		Mod,

		// 比较运算
		/// <summary> 等于 </summary>
		Eq,
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

		// 逻辑运算
		/// <summary> 与 </summary>
		And,
		/// <summary> 或 </summary>
		Or,
		/// <summary> 非(一元) </summary>
		Not,

		// 位运算（可选）
		/// <summary> 按位与 </summary>
		BitAnd,
		/// <summary> 按位或 </summary>
		BitOr,
		/// <summary> 按位异或 </summary>	
		BitXor,
		/// <summary> 按位取反(一元) </summary>
		BitNot,
	}
}
