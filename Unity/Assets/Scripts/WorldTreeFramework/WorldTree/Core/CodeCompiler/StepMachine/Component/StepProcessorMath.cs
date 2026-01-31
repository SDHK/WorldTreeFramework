namespace WorldTree
{

	public partial class StepMachine
	{
		/// <summary> 步骤处理器：数学运算 </summary>
		public StepProcessorMath ProcessorMath;
		/// <summary> 组装处理器：数学运算 </summary>
		public void AddStepProcessorMath() => this.AddComponent(out ProcessorMath);

		/// <summary> 执行数学运算步骤 </summary>
		public void MathOp(StepOpCode op) => ProcessorMath.AddMathOpCode(op);
	}

	/// <summary>
	/// 步骤处理器：数学运算
	/// </summary>
	public class StepProcessorMath : StepProcessor
	{
		/// <summary>
		/// 数学运算数据结构体 
		/// </summary>
		public struct StepDataMath
		{
			/// <summary> 运算操作码 </summary>
			public StepOpCode OpCode;
			/// <summary> 结果地址 </summary>
			public int ResultAddress;
			/// <summary> 参数地址1 </summary>
			public int ParamAddress1;
			/// <summary> 参数地址2 </summary>
			public int ParamAddress2;
			/// <summary> 参数地址3 </summary>
			public int ParamAddress3;
		}

		/// <summary>
		/// 运算操作列表
		/// </summary>
		public UnitList<StepDataMath> dataList;

		/// <summary>
		/// 执行数学运算步骤
		/// </summary>
		private int ExecuteMath(int address, int pointer)
		{
			StepDataMath data = dataList[address];
			// 根据运算类型执行
			switch (data.OpCode)
			{
				case StepOpCode.Conditional:
					ExecuteTrinaryOp(data, pointer); break;
				case StepOpCode.Not:
				case StepOpCode.BitNot:
					ExecuteUnaryOp(data, pointer); break;
				default: ExecuteBinaryOp(data, pointer); break;
			}
			return pointer + 1;
		}

		/// <summary>
		/// 执行三元运算 
		/// </summary>
		private void ExecuteTrinaryOp(StepDataMath data, int pointer)
		{
			// 注意：栈顶是右操作数
			VarValue third = GetParam(data.ParamAddress1);
			VarValue second = GetParam(data.ParamAddress2);
			VarValue first = GetParam(data.ParamAddress3);
			VarValue result = new VarValue();
			switch (data.OpCode)
			{
				case StepOpCode.Conditional: result = first.ToBool() ? second : third; break;
			}
			SetParam(data.ResultAddress, result);
		}

		/// <summary>
		/// 执行二元运算
		/// </summary>
		private void ExecuteBinaryOp(StepDataMath data, int pointer)
		{
			// 注意：栈顶是右操作数
			VarValue right = GetParam(data.ParamAddress1);
			VarValue left = GetParam(data.ParamAddress2);
			VarValue result = new VarValue();
			switch (data.OpCode)
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
			SetParam(data.ResultAddress, result);
		}

		/// <summary>
		/// 执行一元运算
		/// </summary>
		private void ExecuteUnaryOp(StepDataMath data, int pointer)
		{
			VarValue operand = GetParam(data.ParamAddress1);
			VarValue result = new VarValue();
			switch (data.OpCode)
			{
				case StepOpCode.Not: result = !operand.ToBool(); break;
				case StepOpCode.BitNot: result = ~operand; break;
			}
			SetParam(data.ResultAddress, result);
		}

		/// <summary>
		/// 添加数学运算步骤
		/// </summary>
		public void AddMathOpCode(StepOpCode op)
		{
			StepDataMath opData = new();
			opData.OpCode = op;
			switch (op)
			{
				case StepOpCode.Conditional:
					opData.ParamAddress3 = PopParam();
					opData.ParamAddress2 = PopParam();
					opData.ParamAddress1 = PopParam();
					opData.ResultAddress = PushParam();
					break;
				case StepOpCode.Not:
				case StepOpCode.BitNot:
					opData.ParamAddress1 = PopParam();
					opData.ResultAddress = PushParam();
					break;
				default:
					opData.ParamAddress1 = PopParam();
					opData.ParamAddress2 = PopParam();
					opData.ResultAddress = PushParam();
					break;
			}
			dataList.Add(opData);
			AddStep(new StepData(ExecuteMath, dataList.Count - 1));
		}
	}

	public static class StepProcessorMathRule
	{
		class Add : AddRule<StepProcessorMath>
		{
			protected override void Execute(StepProcessorMath self)
			{
				self.Core.PoolGetUnit(out self.dataList);
			}
		}

		class Remove : RemoveRule<StepProcessorMath>
		{
			protected override void Execute(StepProcessorMath self)
			{
				self.dataList.Dispose();
				self.dataList = null;
			}
		}
	}
}
