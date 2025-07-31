/****************************************

* 作者：闪电黑客
* 日期：2024/8/27 11:54

* 描述：

*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;


namespace WorldTree.Server
{


	public static partial class DotNetInitRule
	{


		[NodeRule(nameof(AddRule<DotNetInit>))]
		private static void OnAdd(this DotNetInit self)
		{
			self.Log($"启动！！");

			//self.TestStringConcatPerformance();
			//self.AddComponent(out SerializeTest _);
			//self.AddComponent(out TreeDataTest _);

			//self.AddComponent(out DeepCopyTest _);
		}

		[NodeRule(nameof(EnableRule<DotNetInit>))]
		private static void OnEnable(this DotNetInit self)
		{
			self.Log($"激活！！");
			//self.Root.AddComponent(out InputDeviceManager manager).AddComponent(out InputDriverMouse mouse, manager);
			//MainTime();
			//TestDivisionPrecision();
		}

		[NodeRule(nameof(UpdateTimeRule<DotNetInit>))]
		private static void OnUpdateTime(this DotNetInit self, TimeSpan timeSpan)
		{
			//self.Log($"初始更新！！！{timeSpan.TotalSeconds}");
		}

		[NodeRule(nameof(DisableRule<DotNetInit>))]
		private static void OnDisable(this DotNetInit self)
		{
			self.Log("失活！！");
		}

		[NodeRule(nameof(RemoveRule<DotNetInit>))]
		private static void OnRemove(this DotNetInit self)
		{
			self.Log($"初始关闭！！");
		}



		/// <summary>
		/// 测试定点数除法的精度
		/// </summary>
		public static void TestDivisionPrecision(this DotNetInit self)
		{
			// 测试一系列不同的数值组合
			double[] testNumerators = {
			1.0, 10.0, 100.0, 1000.0,
			0.1, 0.01, 0.001,
			-1.0, -10.0, -100.0
		};

			double[] testDenominators = {
			1.0, 3.0, 7.0, 11.0,
			0.5, 0.25, 2.0,
			-3.0, -7.0
		};

			Console.WriteLine("精度测试结果:");
			Console.WriteLine("被除数\t除数\t浮点除法\t定点数除法\t误差");

			double totalError = 0;
			int testCount = 0;
			double maxError = 0;

			foreach (var a in testNumerators)
			{
				foreach (var b in testDenominators)
				{
					// 跳过除零情况
					if (Math.Abs(b) < 1e-10) continue;

					// 浮点数除法结果
					double floatResult = a / b;

					// 定点数除法
					Fixed fixedA = a;
					Fixed fixedB = b;
					Fixed fixedResult = fixedA / fixedB;

					// 转换回浮点数比较
					double fixedFloatResult = fixedResult;

					// 计算绝对误差
					double error = Math.Abs(floatResult - fixedFloatResult);

					// 相对误差（防止除零）
					double relativeError = Math.Abs(error / (floatResult + 1e-10)) * 100;

					// 记录统计信息
					totalError += error;
					testCount++;
					maxError = Math.Max(maxError, error);

					//// 如果误差超过一定阈值，输出详细信息
					//if (error > 1e-6)
					//{
					//	Console.WriteLine($"{a}\t{b}\t{floatResult:F6}\t{fixedFloatResult:F6}\t{error:E6}");
					//}
					// 输出详细信息
					if (error != 0)
					{
						Console.WriteLine($"{a}\t{b}\t{floatResult}\t{fixedFloatResult}\t{error}");
					}

				}
			}

			// 输出总体统计
			Console.WriteLine("\n总体统计:");
			Console.WriteLine($"测试用例数: {testCount}");
			Console.WriteLine($"平均绝对误差: {totalError / testCount}");
			Console.WriteLine($"最大绝对误差: {maxError}");

		}


		/// <summary>
		/// 耗时测试
		/// </summary>
		static void MainTime()
		{
			const int iterations = 10000000;
			Fixed fixedA = 123.456f;
			Fixed fixedB = 78.9f;
			float floatA = 123.456f;
			float floatB = 78.9f;

			// 测试定点数除法耗时
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < iterations; i++)
			{
				Fixed result = fixedA - fixedB;
			}
			stopwatch.Stop();
			Fixed result1 = fixedA - fixedB;

			Console.WriteLine($"定点数除法耗时: {stopwatch.ElapsedMilliseconds} 毫秒 {(float)result1}");

			// 测试浮点数除法耗时
			stopwatch.Restart();
			for (int i = 0; i < iterations; i++)
			{
				float result = floatA - floatB;
			}
			stopwatch.Stop();
			float result2 = floatA - floatB;

			Console.WriteLine($"浮点数除法耗时: {stopwatch.ElapsedMilliseconds} 毫秒 {result2}");
		}


		[NodeRule(nameof(TestNodeEventRule<Test<T>, I>))]

		static void OnTestNodeEvent<T, I>(this Test<T> self, TestEnum id, I io, List<int> i)
		{

		}

		[NodeRule(nameof(TestNodeEventRule<Test<T>, Type>), true)]
		[RuleSwitch(nameof(OnTestNodeEvent1), nameof(id), TestEnum.Test1)]
		static void OnTestNodeEvent1<T>(this Test<T> self, TestEnum id, Type io, List<int> arg3)
		{
		}

		[NodeRule(nameof(TestNodeEventRule<Test<T>, Type>))]
		[RuleSwitch(nameof(OnTestNodeEvent1), nameof(id), TestEnum.Test1)]
		static void OnTestNodeEvent2<T>(this Test<T> self, TestEnum id, Type io, List<int> arg3)
		{

		}

		/// <summary>
		/// 动态获取到Rule类型
		/// </summary>
		public static Type GetRuleType<T>(this Test<T> self)
		{
			return typeof(OnTestNodeEvent1Rule<Test<T>>);
		}


		class Add : AddRule<DotNetInit>
		{
			public override void OnCreate()
			{
				base.OnCreate();
				RuleType = Core.TypeToCode(typeof(Add));
			}
			protected override void Execute(DotNetInit self)
			{

			}
		}

		/// <summary>
		/// a
		/// </summary>
		static void OnTestNodeEvent2()
		{

		}

		/// <summary>
		/// a
		/// </summary>
		/// <param name="self"></param>
		public static void TestStringConcatPerformance(this DotNetInit self)
		{
			const int iterations = 100_00000;
			const int concatCount = 10;
			string[] values = new string[concatCount];
			for (int i = 0; i < concatCount; i++)
				values[i] = i.ToString();

			//a
			void Test(string name, Action action)
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();
				GC.Collect();
				long gc0 = GC.CollectionCount(0);

				var sw = Stopwatch.StartNew();
				action();
				sw.Stop();

				long gc0After = GC.CollectionCount(0);

				Console.WriteLine($"{name,-18} | Time: {sw.ElapsedMilliseconds,6} ms | GC0: {gc0After - gc0}");
			}

			// 1. + 拼接
			Test("String +", () =>
			{
				for (int i = 0; i < iterations; i++)
				{
					string s = "";
					for (int j = 0; j < concatCount; j++)
						s = s + values[j];
				}
			});

			// 2. 字符串插值
			Test("String { }", () =>
			{
				for (int i = 0; i < iterations; i++)
				{
					string s = $"{values[0]}{values[1]}{values[2]}{values[3]}{values[4]}{values[5]}{values[6]}{values[7]}{values[8]}{values[9]}";
				}
			});

			// 3. StringBuilder
			Test("StringBuilder", () =>
			{
				for (int i = 0; i < iterations; i++)
				{
					var sb = new StringBuilder();
					for (int j = 0; j < concatCount; j++)
						sb.Append(values[j]);
					string s = sb.ToString();
				}
			});
		}


	}


}
