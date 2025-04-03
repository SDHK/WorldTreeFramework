/****************************************

* 作者：闪电黑客
* 日期：2024/9/11 17:35

* 描述：

*/
using System;

namespace WorldTree
{
	public static partial class HotReloadTestRule
	{
		private static OnAdd<HotReloadTest> Add = (self) =>
		{
			self.Log($" 热重载测试！！！");
		};

		private static OnUpdate<HotReloadTest> Update = (self) =>
		{
			self.Log($"热重载2");
		};

		private static OnUpdateTime<HotReloadTest> UpdateTime = (self, timeSpan) =>
		{
			if (Console.KeyAvailable)
			{
				var key = Console.ReadKey(intercept: true);
				if (key.Key == ConsoleKey.A)
				{
					self.Log($"键盘输入 'A' 键 , 热重载！！！");
					self.World.AddComponent(out CodeLoader _).HotReload();
				}
			}
			//self.Log($"初始更新！！！{timeSpan.TotalSeconds}");
		};

	}

}