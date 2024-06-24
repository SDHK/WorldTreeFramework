using System.Collections.Generic;

namespace WorldTree
{

	delegate void aOnTestEvent<T>(T self) where T : INode;

	public struct AT
	{
		public const int ACC_BAA = 1;
		public int A;
	}


	/// <summary>
	/// 测试节点
	/// </summary>
	public partial class DotNetTestNode : Node, ComponentOf<INode>
		, AsAwake
		, AsCurveEvaluate
		, AsTestEvent
	{
		private int a;

		protected int cbacc1;

		private const int A_ACC_BAA = 1;


		public List<int> NtList { get; }
		public List<int> NtLi1LisList;

		public int Ab { get; }

		public int AestValue;
		public int ABaaLis;

		public void AVvv()
		{

		}
	}

	public static partial class DotNetTestNodeRule
	{
		private static OnTestEvent<DotNetTestNode> testEvent = (self) =>
		{
			self.Log("测试事件！！！");
		};

		private static OnUpdate<DotNetTestNode> update = (self) =>
		{
			//全局调用
			self.Core.GetOrNewGlobalRuleActuator(out GlobalRuleActuator<TestEvent> act).Send();

			//指定调用
			self.TestEvent();

			self.Log($"初始更新！！！");
		};

		private static OnUpdateTime<DotNetTestNode> updateTime = (self, timeSpan) =>
		{
			self.Log($"初始更新！！！{timeSpan.TotalSeconds}");
		};

		private static OnCurveEvaluate<DotNetTestNode> onCurveEvaluate = (self, time) =>
		{
			self.Log($"曲线更新！！！{time}");
			return time;
		};

		private static OnEnable<DotNetTestNode> enable1 = (self) =>
		{
			self.Log("激活1！！");
		};

		private static OnEnable<DotNetTestNode> enable2 = (self) =>
		{
			self.Log("激活2！！");
		};

		private static OnEnable<DotNetTestNode> enable3 = (self) =>
		{
			self.Log("激活3！！");
		};

		private static OnDisable<DotNetTestNode> disable = (self) =>
		{
			self.Log("失活！！");
		};

		private static OnNew<DotNetTestNode> new1 = (self) =>
		{
			self.Log("新建1！！");
		};

		private static OnNew<DotNetTestNode> new2 = (self) =>
		{
			self.Log("新建2！！");
		};

		private static OnAdd<DotNetTestNode> add = (self) =>
		{
			self.Log(" 初始化！！！");
		};

		private static OnRemove<DotNetTestNode> remove = (self) =>
		{
			self.Log($"初始关闭！！");
		};
	}
}