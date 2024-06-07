namespace WorldTree
{
	/// <summary>
	/// 测试节点
	/// </summary>
	public partial class DotNetTestNode : Node, ComponentOf<INode>
		, AsAwake
		, AsCurveEvaluate
	{
		public int TestValue;
	}


	public static partial class DotNetTestNodeRule
	{
		static OnCurveEvaluate<DotNetTestNode> OnCurveEvaluate = (self, time) =>
		{
			self.Log($"曲线更新！！！{time}");
			return time;
		};

		static OnEnable<DotNetTestNode> Enable1 = (self) =>
		{
			self.Log("激活1！！");
		};

		static OnEnable<DotNetTestNode> Enable2 = (self) =>
		{
			self.Log("激活2！！");
		};

		static OnEnable<DotNetTestNode> Enable3 = (self) =>
		{
			self.Log("激活3！！");
		};

		static OnDisable<DotNetTestNode> Disable = (self) =>
		{
			self.Log("失活！！");
		};

		static OnNew<DotNetTestNode> New1 = (self) =>
		{
			self.Log("新建1！！");
		};
		static OnNew<DotNetTestNode> New2 = (self) =>
		{
			self.Log("新建2！！");
		};
		static OnAdd<DotNetTestNode> Add = (self) =>
		{
			self.Log(" 初始化！！！");
		};
		static OnUpdate<DotNetTestNode> update = (self) =>
		{
			self.Log($"初始更新！！！");
		};
		static OnUpdateTime<DotNetTestNode> updateTime = (self, timeSpan) =>
		{
			self.Log($"初始更新！！！{timeSpan.TotalSeconds}");
		};

		static OnRemove<DotNetTestNode> Remove = (self) =>
		{
			self.Log($"初始关闭！！");
		};

	}

}