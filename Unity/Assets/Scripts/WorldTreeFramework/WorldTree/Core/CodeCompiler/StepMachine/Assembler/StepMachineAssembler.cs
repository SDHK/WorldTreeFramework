namespace WorldTree
{
	/// <summary>
	/// 步骤执行器组装器  
	/// </summary>
	public class StepMachineAssembler : Node
	{
		/// <summary>
		/// 组装步骤执行器 
		/// </summary>
		public static void Assemble(StepMachine stepMachine, UnitList<StepAssemblyData> datas)
		{
			for (int i = 0; i < datas.Count; i++)
			{
				StepAssemblyData data = datas[i];

				switch (data.OpCode)
				{
					case StepOpCode.None:
						break;
					case StepOpCode.Add:
					case StepOpCode.Sub:
					case StepOpCode.Mul:
					case StepOpCode.Div:
					case StepOpCode.Mod:
					case StepOpCode.Eq:
					case StepOpCode.NotEq:
					case StepOpCode.Greater:
					case StepOpCode.GreaterEq:
					case StepOpCode.Less:
					case StepOpCode.LessEq:
					case StepOpCode.Conditional:
					case StepOpCode.And:
					case StepOpCode.Or:
					case StepOpCode.Not:
					case StepOpCode.BitAnd:
					case StepOpCode.BitOr:
					case StepOpCode.BitXor:
					case StepOpCode.BitNot:
					case StepOpCode.BitShiftLeft:
					case StepOpCode.BitShiftRight:
						stepMachine.MathOp(data.OpCode); break;
					case StepOpCode.PushLiteral:
						stepMachine.PushLiteral(data.Literal);
						break;
					case StepOpCode.PopRecycle:
						stepMachine.PopParam();
						break;
					case StepOpCode.PushVar:
						break;
					case StepOpCode.PopVar:
						break;
					case StepOpCode.Dup:
						stepMachine.Dup();
						break;
					case StepOpCode.IfPop:
						stepMachine.IfPop();
						break;
					case StepOpCode.Else:
						stepMachine.Else();
						break;
					case StepOpCode.IfEnd:
						stepMachine.IfEnd();
						break;
					case StepOpCode.JumpLabel:
						break;
					case StepOpCode.Jump:
						break;
					case StepOpCode.LoopEnter:
						stepMachine.LoopEnter();
						break;
					case StepOpCode.LoopEnd:
						stepMachine.LoopEnd();
						break;
					case StepOpCode.LoopCheckPop:
						stepMachine.LoopCheckPop();
						break;
					case StepOpCode.Break:
						stepMachine.Break();
						break;
					case StepOpCode.Continue:
						stepMachine.Continue();
						break;
					default:
						break;
				}
			}
		}
	}
}
