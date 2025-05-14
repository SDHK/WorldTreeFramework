/****************************************

* 作者：闪电黑客
* 日期：2024/9/11 17:35

* 描述：

*/
using System;

namespace WorldTree.Server
{
	public static partial class HotReloadTestRule
	{
		[NodeRule(nameof(AddRule<HotReloadTest>))]
		private static void OnAdd(this HotReloadTest self)
		{
			self.Log($" 热重载测试！！！");

		}

		[NodeRule(nameof(UpdateRule<HotReloadTest>))]
		private static void OnUpdate(this HotReloadTest self)
		{
			self.Log($"热重载2");

		}

		[NodeRule(nameof(UpdateTimeRule<HotReloadTest>))]
		private static void OnUpdateTime(this HotReloadTest self, TimeSpan arg1)
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
		}
	}

}