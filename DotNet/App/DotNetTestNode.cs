using System;

namespace WorldTree
{

	/// <summary>
	/// 测试节点
	/// </summary>
	public partial class DotNetTestNode : Node, ComponentOf<INode>
		, AsComponentBranch
		, AsAwake
		, AsCurveEvaluate
	{ }

	/// <summary>
	/// DotNetTestNodeRule
	/// </summary>
	public static partial class DotNetTestNodeRule
	{
		private static OnEnable<DotNetTestNode> Enable1 = (self) =>
		{
			self.Log("激活！！");
		};

		private static OnAdd<DotNetTestNode> Add = (self) =>
		{
			self.Log(" 初始化！！！");
			self.AddComponent(out SerializeTest _);
		};

		private static OnUpdate<DotNetTestNode> Update = (self) =>
		{
			self.Log($"初始更新！！！");
		};

		// 需要修改代码生成,Rule遍历执行由Forech改为For

		private static OnUpdateTime<DotNetTestNode> UpdateTime = (self, timeSpan) =>
		{
			//检测键盘输入a
			if (Console.KeyAvailable)
			{
				var key = Console.ReadKey(intercept: true);
				if (key.Key == ConsoleKey.A)
				{
					self.Log($"键盘输入 'a' 键！！！");
					//self.Root.AddComponent(out CodeLoader _).HotReload();
				}
			}
			//self.Log($"初始更新！！！{timeSpan.TotalSeconds}");
		};

		private static OnDisable<DotNetTestNode> Disable = (self) =>
		{
			self.Log("失活！！");
		};

		private static OnRemove<DotNetTestNode> Remove = (self) =>
		{
			self.Log($"初始关闭！！");
		};
	}
}