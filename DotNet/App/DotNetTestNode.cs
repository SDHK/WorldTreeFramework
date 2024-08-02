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
			//self.Log($"初始更新！！！");
		};

		private static OnUpdateTime<DotNetTestNode> UpdateTime = (self, timeSpan) =>
		{
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