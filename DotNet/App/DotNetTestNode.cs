using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WorldTree
{
	/// <summary>
	/// 测试节点
	/// </summary>
	public partial class DotNetTestNode : Node, ComponentOf<INode>
		, AsAwake
		, AsCurveEvaluate
		, AsComponentBranch
	{

	}

	/// <summary>
	/// DotNetTestNodeRule
	/// </summary>
	public static partial class DotNetTestNodeRule
	{
		private static OnUpdate<DotNetTestNode> Update = (self) =>
		{
			self.Log($"初始更新！！！");
		};

		private static OnUpdateTime<DotNetTestNode> UpdateTime = (self, timeSpan) =>
		{
			self.Log($"初始更新！！！{timeSpan.TotalSeconds}");
		};

		private static OnCurveEvaluate<DotNetTestNode> OnCurveEvaluate = (self, time) =>
		{
			self.Log($"曲线更新！！！{time}");
			return time;
		};

		private static OnEnable<DotNetTestNode> Enable1 = (self) =>
		{
			self.Log("激活1！！");
		};

		private static OnEnable<DotNetTestNode> Enable2 = (self) =>
		{
			self.Log("激活2！！");
		};

		private static OnEnable<DotNetTestNode> Enable3 = (self) =>
		{
			self.Log("激活3！！");
		};

		private static OnDisable<DotNetTestNode> Disable = (self) =>
		{
			self.Log("失活！！");
		};

		private static OnNew<DotNetTestNode> New1 = (self) =>
		{
			self.Log("新建1！！");
		};

		private static OnNew<DotNetTestNode> New2 = (self) =>
		{
			self.Log("新建2！！");
		};

		private static OnAdd<DotNetTestNode> Add = (self) =>
		{
			self.Log(" 初始化！！！");
		};

		private static OnRemove<DotNetTestNode> Remove = (self) =>
		{
			self.Log($"初始关闭！！");
		};
	}
}