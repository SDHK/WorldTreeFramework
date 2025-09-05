using UnityEngine;

namespace VM
{
	public class TaskExecutorExample : MonoBehaviour
	{
		public TaskExecutor executor = new TaskExecutor();
		private void Start()
		{
			RunSimpleExample();
		}

		public void Update()
		{
			executor.Update();
		}

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
				.PushValue(3)     // int -> VarValue 隐式转换
				.MethodCall("Add", 2)  // Add(2, 3) -> 5
				.MethodCall("AddB", 2)  // Add(1, 5) -> 6

				// 将结果赋值给变量a
				.Event(() =>
				{
					var result = executor.PopParameter();
					executor.SetVariable("a", result);
					Debug.Log($"a = {result}"); // 输出: a = 6
				});

			// 执行
			executor.Run();
		}
	}
}