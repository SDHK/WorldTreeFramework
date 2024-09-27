/****************************************

* 作者：闪电黑客
* 日期：2024/8/27 11:54

* 描述：

*/
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;


namespace WorldTree
{
	

	/// <summary>
	/// DotNetTestNodeRule
	/// </summary>
	public static partial class DotNetInitRule
	{
		private static OnEnable<DotNetInit> Enable1 = (self) =>
		{
			self.TestRule(1.0f, "字符串");


			self.Log($"激活！！");
		};


		private static OnTestRule<DotNetInit> TestRule = (self, f, s) =>
		{
			self.Log($"测试法则{f} {s}");
		};


		private static OnAdd<DotNetInit> Add = (self) =>
		{
			//self.AddComponent(out SerializeTest _);
			self.AddComponent(out TreeDataTest _);
		};

		private static OnUpdateTime<DotNetInit> UpdateTime = (self, timeSpan) =>
		{
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
