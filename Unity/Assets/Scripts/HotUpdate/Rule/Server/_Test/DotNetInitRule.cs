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
			self.AddComponent(out SerializeTest _);
		};

		private static OnUpdate<DotNetInit> Update = (self) =>
		{
			self.Log($"初始更新！！！热重载4");
		};

		// 需要修改代码生成,Rule遍历执行由Forech改为For

		private static OnUpdateTime<DotNetInit> UpdateTime = (self, timeSpan) =>
		{
			//检测键盘输入a
			if (Console.KeyAvailable)
			{
				var key = Console.ReadKey(intercept: true);
				if (key.Key == ConsoleKey.D)
				{
					self.Log($"键盘输入 'D' 键！！！");
					self.Root.AddComponent(out CodeLoader _).HotReload();
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
