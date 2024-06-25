using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WorldTree
{

	/// <summary>
	/// 测试接口
	/// </summary>
	public interface AITa<A>
	{
		/// <summary>
		/// 测试aaa
		/// </summary>
		public int AAA_A { get; set; }
		/// <summary>
		/// a
		/// </summary>
		public void Tast();

	}

	/// <summary>
	/// 测试节点
	/// </summary>
	public class Test1 : Node
		, AsAwake
		 , ComponentOf<INode>
	{
		/// <summary>
		/// a
		/// </summary>
		public int a;

		/// <summary>
		/// B
		/// </summary>
		public int B;
	}


	/// <summary>
	/// 测试节点规则
	/// </summary>
	public static partial class DotNetTestNodeRule
	{
		/// <summary>
		/// 测试扩展方法
		/// </summary>
		public static void Test(this Test1 self, DotNetTestNode dotNetTestNode)
		{
			// a获取
			var a = dotNetTestNode.AddComponent(out Test1 _).a;

			// b获取
			var b = dotNetTestNode.AddComponent(out Test1 _).B;
		}

		private static OnAdd<Test1> AddTest = delegate (Test1 self) 
		{
			// a获取
			//var a = self.AddComponent(out Test1 _).a;
			var a = self.a;
		};

		private class AddT : AddRule<Test1>
		{
			protected override void Execute(Test1 self)
			{
				// a
				var a = self.a;
			}
		}


	}


	/// <summary>
	/// 测试节点
	/// </summary>
	public partial class DotNetTestNode : Node, ComponentOf<INode>
		, AsAwake
		, AsCurveEvaluate
		, AsTestEvent
		, AsComponentBranch
	{
		/// <summary>
		/// a
		/// </summary>
		public int a;

		/// <summary>
		/// a
		/// </summary>
		public int At { get; }

		//public int At =>this.TryGetComponent(out Test1 test1) ? test1.a : 0;

		/// <summary>
		/// 注释
		/// </summary>
		public List<int> NtList;

		/// <summary>
		/// 字典注释
		/// </summary>
		public Dictionary<int, string> KeysDicDict;

		/// <summary>
		/// 数组注释
		/// </summary>
		public int[] Ints;

		/// <summary>
		/// 注释
		/// </summary>
		public Queue<int> intsQueue;

		/// <summary>
		/// 注释
		/// </summary>
		private Stack<int> intsStack;

		/// <summary>
		/// a
		/// </summary>
		public HashSet<int> IntsHash;

		/// <summary>
		/// a
		/// </summary>
		public void OuterMethod()
		{
			// a
			int LocalFunction(int x, int y)
			{
				return x + y;
			}

			// a
			int result = LocalFunction(5, 3);
			Console.WriteLine(result); // 输出: 8
		}

	}

	/// <summary>
	/// DotNetTestNodeRule
	/// </summary>
	public static partial class DotNetTestNodeRule
	{
		private static OnTestEvent<DotNetTestNode> TestEvent = (self) =>
		{
			self.Log("测试事件！！！");
		};

		private static OnUpdate<DotNetTestNode> Update = (self) =>
		{
			// a 
			var a = self.NtList;

			//全局调用
			self.Core.GetOrNewGlobalRuleActuator(out GlobalRuleActuator<TestEvent> act).Send();

			//指定调用
			self.TestEvent();

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