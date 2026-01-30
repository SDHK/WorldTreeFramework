namespace WorldTree
{

	public partial class StepMachine
	{
		/// <summary> 步骤处理器：数学运算 </summary>
		public StepProcessorMath ProcessorMath;
		/// <summary> 组装处理器：数学运算 </summary>
		public void AddStepProcessorMath() => this.AddComponent(out ProcessorMath);

		/// <summary> 执行数学运算步骤 </summary>
		public void MathOp(StepOpCode op) => ProcessorMath.AddMathOp(op);
	}

	/// <summary>
	/// 步骤处理器：数学运算
	/// </summary>
	public class StepProcessorMath : StepProcessor
	{
		/// <summary>
		/// 运算操作列表
		/// </summary>
		public UnitList<StepOpCode> operationList;

		/// <summary>
		/// 执行数学运算步骤
		/// </summary>
		private int ExecuteMath(int address, int pointer)
		{
			StepOpCode op = operationList[address];
			// 根据运算类型执行
			switch (op)
			{
				case StepOpCode.Not:
				case StepOpCode.BitNot:
					ExecuteUnaryOp(op, pointer); break;
				default: ExecuteBinaryOp(op, pointer); break;
			}
			return pointer + 1;
		}

		/// <summary>
		/// 执行二元运算
		/// </summary>
		private void ExecuteBinaryOp(StepOpCode op, int pointer)
		{
			// 注意：栈顶是右操作数
			VarValue right = Machine.Pop();
			VarValue left = Machine.Pop();
			VarValue result = new VarValue();
			switch (op)
			{
				case StepOpCode.Add: result = left + right; break;
				case StepOpCode.Sub: result = left - right; break;
				case StepOpCode.Mul: result = left * right; break;
				case StepOpCode.Div:
					if (right == 0) this.LogError($"除零错误：{left} / {right}，位置：步骤{pointer}");
					result = left / right; break;
				case StepOpCode.Mod: result = left % right; break;
				case StepOpCode.Eq: result = left == right; break;
				case StepOpCode.NotEq: result = left != right; break;
				case StepOpCode.Greater: result = left > right; break;
				case StepOpCode.GreaterEq: result = left >= right; break;
				case StepOpCode.Less: result = left < right; break;
				case StepOpCode.LessEq: result = left <= right; break;
				case StepOpCode.And: result = left.ToBool() && right.ToBool(); break;
				case StepOpCode.Or: result = left.ToBool() || right.ToBool(); break;
				case StepOpCode.BitAnd: result = left & right; break;
				case StepOpCode.BitOr: result = left | right; break;
				case StepOpCode.BitXor: result = left ^ right; break;
				case StepOpCode.BitShiftLeft: result = left << right.ToInt(); break;
				case StepOpCode.BitShiftRight: result = left >> right.ToInt(); break;
			}
			Machine.Push(result);
		}

		/// <summary>
		/// 执行一元运算
		/// </summary>
		private void ExecuteUnaryOp(StepOpCode op, int pointer)
		{
			VarValue operand = Machine.Pop();
			VarValue result = new VarValue();
			switch (op)
			{
				case StepOpCode.Not: result = !operand.ToBool(); break;
				case StepOpCode.BitNot: result = ~operand; break;
			}
			Machine.Push(result);
		}

		/// <summary>
		/// 添加数学运算步骤
		/// </summary>
		public void AddMathOp(StepOpCode op)
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
