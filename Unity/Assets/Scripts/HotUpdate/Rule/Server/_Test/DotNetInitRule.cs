using System;


namespace WorldTree
{
	/// <summary>
	/// DotNetTestNodeRule
	/// </summary>
	public static partial class DotNetInitRule
	{
		private static OnEnable<DotNetInit> Enable1 = (self) =>
		{
			self.Log("激活！！");
		};

		private static OnAdd<DotNetInit> Add = (self) =>
		{
			self.Log(" 初始化！！！");

			//self.AddComponent(out SerializeTest _);

			self.action = self.TestAction;
		};

		private static void TestAction(this DotNetInit self)
		{
			self.Log($"测试委托2");
		}

		private static OnUpdate<DotNetInit> Update = (self) =>
		{
			self.Log($"初始更新！！！热重载2");
		};

		private static OnUpdateTime<DotNetInit> UpdateTime = (self, timeSpan) =>
		{
			if (Console.KeyAvailable)
			{
				var key = Console.ReadKey(intercept: true);
				if (key.Key == ConsoleKey.A)
				{
					self.Log($"键盘输入 'A' 键 , 热重载！！！");
					self.Root.AddComponent(out CodeLoader _).HotReload();
				}
				else if (key.Key == ConsoleKey.Q)
				{
					self.Log($"键盘输入 'Q' 键 , 执行委托！！！");
					self.action.Invoke();
				}
			}


			//self.Log($"初始更新！！！{timeSpan.TotalSeconds}");
		};

		private static OnDisable<DotNetInit> Disable = (self) =>
		{
			self.Log("失活！！");
		};

		private static OnRemove<DotNetInit> Remove = (self) =>
		{
			self.Log($"初始关闭！！");
		};
	}


}
