//namespace WorldTree
//{
//	public static partial class StepMathParserTestRule
//	{
//		class AwakeRule : AwakeRule<StepMathParserTest>
//		{
//			protected override void Execute(StepMathParserTest self)
//			{
//				self.AddTemp(out self.stepMachine);
//				self.stepMachine.AddStepProcessorMath();
//				self.stepMachine.AddStepProcessorLiteral();

//				// 创建解析器
//				self.AddTemp(out StepMathParser parser);

//				// 测试表达式
//				string[] expressions = new string[]
//				{
//					"100 + 50",                          // 简单加法: 150
//					"(100 + 50) / 3",                    // 括号和除法: 50
//					"(100 + 50) / 3 * 2 - 10 % 3",      // 复杂表达式: 99
//					"10 * (5 + 3) - 4 / 2",              // 混合运算: 78
//					"-5 + 10",                           // 负数: 5
//					"-(10 - 3) * 2",                     // 负号和括号: -14
//				};

//				double[] expectedResults = new double[]
//				{
//					150,
//					50,
//					99,
//					78,
//					5,
//					-14,
//				};

//				self.Log("=== StepMathParser 测试开始 ===");

//				for (int i = 0; i < expressions.Length; i++)
//				{
//					string expr = expressions[i];
//					double expected = expectedResults[i];

//					self.Log($"\n测试 {i + 1}: {expr}");
//					self.Log($"预期结果: {expected}");

//					// 解析表达式生成汇编数据
//					self.Core.PoolGetUnit(out UnitList<StepAssemblyData> datas);
//					parser.Parse(expr, datas);

//					// 打印生成的汇编代码
//					self.Log("生成的汇编代码:");
//					for (int j = 0; j < datas.Count; j++)
//					{
//						var data = datas[j];
//						if (data.OpCode == StepOpCode.PushLiteral)
//						{
//							self.Log($"  [{j}] {data.OpCode} {data.Literal.ToLong()}");
//						}
//						else
//						{
//							self.Log($"  [{j}] {data.OpCode}");
//						}
//					}

//					// 清空并重新组装虚拟机
//					self.stepMachine.StepList.Clear();
//					self.stepMachine.ParamAddress = 0;
//					self.stepMachine.MaxCapacity = 0;

//					// 组装并执行
//					StepMachineAssembler.Assemble(self.stepMachine, datas);
//					self.stepMachine.Run();

//					// 执行所有步骤
//					while (self.stepMachine.Pointer != -1 && self.stepMachine.isRun)
//					{
//						self.stepMachine.Update();
//					}

//					// 获取结果
//					if (self.stepMachine.ParamAddress > 0)
//					{
//						long result = self.stepMachine.ParamList[0].ToLong();
//						self.Log($"实际结果: {result}");

//						if (result == (long)expected)
//						{
//							self.Log("✓ 测试通过");
//						}
//						else
//						{
//							self.Log($"✗ 测试失败！期望 {expected}，实际 {result}");
//						}
//					}
//					else
//					{
//						self.Log("✗ 错误：栈为空");
//					}

//					// 回收资源
//					self.Core.PoolRecycle(datas);
//				}

//				self.Log("\n=== StepMathParser 测试结束 ===");
//			}
//		}
//	}
//}
