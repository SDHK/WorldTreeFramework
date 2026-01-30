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
		public UnitList<StepOperationCode> operationList;

		/// <summary>
		/// 执行数学运算步骤
		/// </summary>
		private int ExecuteMath(int address, int pointer)
		{
			StepOperationCode op = operationList[address];
			// 根据运算类型执行
			switch (op)
			{
				case StepOperationCode.Not:
				case StepOperationCode.BitNot:
					ExecuteUnaryOp(op, pointer); break;
				default: ExecuteBinaryOp(op, pointer); break;
			}
			return pointer + 1;
		}

		/// <summary>
		/// 执行二元运算
		/// </summary>
		private void ExecuteBinaryOp(StepOperationCode op, int pointer)
		{
			// 注意：栈顶是右操作数
			VarValue right = Machine.Pop();
			VarValue left = Machine.Pop();
			VarValue result = new VarValue();
			switch (op)
			{
				case StepOperationCode.Add: result = left + right; break;
				case StepOperationCode.Sub: result = left - right; break;
				case StepOperationCode.Mul: result = left * right; break;
				case StepOperationCode.Div:
					if (right == 0) this.LogError($"除零错误：{left} / {right}，位置：步骤{pointer}");
					result = left / right; break;
				case StepOperationCode.Mod: result = left % right; break;
				case StepOperationCode.Eq: result = left == right; break;
				case StepOperationCode.NotEq: result = left != right; break;
				case StepOperationCode.Greater: result = left > right; break;
				case StepOperationCode.GreaterEq: result = left >= right; break;
				case StepOperationCode.Less: result = left < right; break;
				case StepOperationCode.LessEq: result = left <= right; break;
				case StepOperationCode.And: result = left.ToBool() && right.ToBool(); break;
				case StepOperationCode.Or: result = left.ToBool() || right.ToBool(); break;
				case StepOperationCode.BitAnd: result = left & right; break;
				case StepOperationCode.BitOr: result = left | right; break;
				case StepOperationCode.BitXor: result = left ^ right; break;
				case StepOperationCode.BitShiftLeft: result = left << right.ToInt(); break;
				case StepOperationCode.BitShiftRight: result = left >> right.ToInt(); break;
			}
			Machine.Push(result);
		}

		/// <summary>
		/// 执行一元运算
		/// </summary>
		private void ExecuteUnaryOp(StepOperationCode op, int pointer)
		{
			VarValue operand = Machine.Pop();
			VarValue result = new VarValue();
			switch (op)
			{
				case StepOperationCode.Not: result = !operand.ToBool(); break;
				case StepOperationCode.BitNot: result = ~operand; break;
			}
			Machine.Push(result);
		}

		/// <summary>
		/// 添加数学运算步骤
		/// </summary>
		public void AddMathOp(StepOperationCode op)
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
}
