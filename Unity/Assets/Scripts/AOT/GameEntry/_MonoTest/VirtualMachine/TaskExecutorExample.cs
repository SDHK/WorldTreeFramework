using Sirenix.OdinInspector;
using System.IO;
using UnityEngine;

namespace VM
{
	public class TaskExecutorExample : MonoBehaviour
	{
		/// <summary>
		/// 虚拟机
		/// </summary>
		TaskExecutor executor = new TaskExecutor();

		/// <summary>
		/// 汇编语法解析器
		/// </summary>
		AssemblySyntaxParser syntaxParser = new();

		/// <summary>
		/// 代码文件路径
		/// </summary>
		public string FilePath;

		[Button("Run")]
		private void Run()
		{
			string code = File.ReadAllText(FilePath);
			executor.Clear();
			syntaxParser.Parse(executor, code);
			executor.Run();
		}

		public void Update()
		{
			executor.Update();
		}




		/// <summary>
		/// 直接使用虚拟机执行器编写简单示例
		/// </summary>
		public void RunSimpleExample()
		{
			// 构建嵌套方法调用: a = Add(1, Add(2, 3))

			executor
				// 定义Add方法 - 利用VarValue隐式转换
				.MethodDefine("Add", "x", "y")
					.Event(() =>
					{
						long x = executor.GetVariable("x").ToLong();
						long y = executor.GetVariable("y").ToLong();
						Debug.Log($"x {x} y{y}");
						// 直接使用隐式转换和ToLong()
						long sum = x + y;
						executor.SetVariable("result", sum); // 隐式转换 long -> VarValue
						Debug.Log($"result = {executor.GetVariable("result").ToLong()}"); // 输出: a = 6
					})
					.PushVariable("result")
				.MethodEnd()
				.MethodDefine("AddB", "x", "y")
					.Event(() =>
					{
						long x = executor.GetVariable("x").ToLong();
						long y = executor.GetVariable("y").ToLong();
						Debug.Log($"x {x} y{y}");
						// 直接使用隐式转换和ToLong()
						double sum = (float)x / (float)y;
						executor.SetVariable("result", sum); // 隐式转换 long -> VarValue
						Debug.Log($"result = {executor.GetVariable("result")}"); // 输出: a = 6
					})
					.PushVariable("result")
				.MethodEnd()

				// 执行嵌套调用 - 使用隐式转换简化代码
				.PushValue(1)     // int -> VarValue 隐式转换
				.PushValue(2)     // int -> VarValue 隐式转换  
				.PushValue(7)     // int -> VarValue 隐式转换
				.MethodCall("Add")  // Add(2, 3) -> 5
				.MethodCall("AddB")  // Add(1, 5) -> 6
				.PopVariable("result")
				// 将结果赋值给变量a
				.Event(() =>
				{
					var result = executor.GetVariable("result");
					Debug.Log($"a = {result}"); // 输出: a = 6
				});

			// 执行
			executor.Run();
		}
	}

}