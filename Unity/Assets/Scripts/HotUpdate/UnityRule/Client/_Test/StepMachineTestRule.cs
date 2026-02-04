namespace WorldTree
{
	public static partial class StepMachineTestRule
	{
		class AwakeRule : AwakeRule<StepMachineTest>
		{
			protected override void Execute(StepMachineTest self)
			{
				self.AddTemp(out self.stepMachine);
				self.stepMachine.AddStepProcessorIfElse();
				self.stepMachine.AddStepProcessorLiteral();
				self.stepMachine.AddStepProcessorEvent();
				self.stepMachine.AddStepProcessorLoop();
				self.stepMachine.AddStepProcessorMath();

				self.Core.PoolGetUnit(out UnitList<StepAssemblyData> datas);

				//表达式：(100 + 50) / 3 * 2 - 10 % 3
				// 1. 计算 100 + 50 = 150
				datas.Add(new(StepOpCode.PushLiteral, (VarValue)100L));
				datas.Add(new(StepOpCode.PushLiteral, (VarValue)50L));
				datas.Add(new(StepOpCode.Add));

				// 2. 除以 3: 150 / 3 = 50
				datas.Add(new(StepOpCode.PushLiteral, (VarValue)3.0));
				datas.Add(new(StepOpCode.Div));

				// 3. 乘以 2: 50 * 2 = 100
				datas.Add(new(StepOpCode.PushLiteral, (VarValue)2L));
				datas.Add(new(StepOpCode.Mul));

				// 4. 计算 10 % 3 = 1
				datas.Add(new(StepOpCode.PushLiteral, (VarValue)10L));
				datas.Add(new(StepOpCode.PushLiteral, (VarValue)3L));
				datas.Add(new(StepOpCode.Mod));

				// 5. 减法: 100 - 1 = 99
				datas.Add(new(StepOpCode.Sub));

				//if (result > 50) { result = result * 2 } else { result = result + 10 }

				self.Log("=== 条件分支计算 ===");
				self.Log("if (result > 50) { result = result * 2 } else { result = result + 10 }");

				// 6. 复制栈顶值用于比较
				datas.Add(new(StepOpCode.Dup));
				datas.Add(new(StepOpCode.PushLiteral, (VarValue)50L));
				datas.Add(new(StepOpCode.Greater)); // 判断 result > 50

				// 7. If 分支开始
				datas.Add(new(StepOpCode.IfPop));

				// 8. True 分支: result * 2
				datas.Add(new(StepOpCode.PushLiteral, (VarValue)2L));
				datas.Add(new(StepOpCode.Mul)); // result = 99 * 2 = 198

				// 9. Else 分支
				datas.Add(new(StepOpCode.Else));

				// 10. False 分支: result + 10 (不会执行)
				datas.Add(new(StepOpCode.PushLiteral, (VarValue)10L));
				datas.Add(new(StepOpCode.Add));

				// 11. If 结束
				datas.Add(new(StepOpCode.IfEnd));

				// ===== 位运算示例 =====
				// result = result & 255 (保留低8位)
				self.Log("=== 位运算 ===");
				self.Log("result = result & 255");

				datas.Add(new(StepOpCode.PushLiteral, (VarValue)255L));
				datas.Add(new(StepOpCode.BitAnd)); // 198 & 255 = 198

				// ===== 逻辑运算示例 =====
				// isValid = (result > 100) && (result < 300)
				self.Log("=== 逻辑运算 ===");
				self.Log("isValid = (result > 100) && (result < 300)");

				// 计算 result > 100
				datas.Add(new(StepOpCode.PushLiteral, (VarValue)100L));
				datas.Add(new(StepOpCode.Greater)); // 198 > 100 = true

				// 计算 result < 300 (需要重新压入result值)
				datas.Add(new(StepOpCode.PushLiteral, (VarValue)198L)); // 手动压入当前结果
				datas.Add(new(StepOpCode.PushLiteral, (VarValue)300L));
				datas.Add(new(StepOpCode.Less)); // 198 < 300 = true

				// 逻辑与运算
				datas.Add(new(StepOpCode.And)); // true && true = true



				// 组装虚拟机
				StepMachineAssembler.Assemble(self.stepMachine, datas);
				self.stepMachine.Run();
			}
		}

		[NodeRule(nameof(UpdateRule<StepMachineTest>))]
		private static void OnUpdateRule(this StepMachineTest self)
		{
			self.stepMachine.Update();
			string log = $"StepMachineTest Update [{self.stepMachine.Pointer - 1}]:";
			for (int i = 0; i < self.stepMachine.ParamList.Count; i++)
			{
				log += $" {self.stepMachine.ParamList[i].ToLong()}";
			}
			self.Log(log);
		}
	}
}
