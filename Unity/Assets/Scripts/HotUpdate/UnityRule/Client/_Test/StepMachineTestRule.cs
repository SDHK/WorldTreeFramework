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
				// 计算: (10 + 5) * 2 - 3 = 27
				// 使用后缀表达式(逆波兰表示法): 10 5 + 2 * 3 -

				// 步骤 1: 压入 10
				datas.Add(new(StepOpCode.PushLiteral, (VarValue)10L));

				// 步骤 2: 压入 5
				datas.Add(new(StepOpCode.PushLiteral, (VarValue)5L));

				// 步骤 3: 执行加法 (10 + 5 = 15)
				datas.Add(new(StepOpCode.Add));

				// 步骤 4: 压入 2
				datas.Add(new(StepOpCode.PushLiteral, (VarValue)2L));

				// 步骤 5: 执行乘法 (15 * 2 = 30)
				datas.Add(new(StepOpCode.Mul));

				// 步骤 6: 压入 3
				datas.Add(new(StepOpCode.PushLiteral, (VarValue)3L));

				// 步骤 7: 执行减法 (30 - 3 = 27)
				datas.Add(new(StepOpCode.Sub));
				StepMachineAssembler.Assemble(self.stepMachine, datas);
				self.stepMachine.Run();
			}
		}

		[NodeRule(nameof(UpdateRule<StepMachineTest>))]
		private static void OnUpdateRule(this StepMachineTest self)
		{
			self.stepMachine.Update();
			string log = "StepMachineTest Update:";
			for (int i = 0; i < self.stepMachine.ParamList.Count; i++)
			{
				log += $" {self.stepMachine.ParamList[i].ToLong()}";
			}
			self.Log(log);
		}
	}
}
