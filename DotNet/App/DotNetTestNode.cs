﻿using System.Collections.Generic;

namespace WorldTree
{
	/// <summary> 
	/// 
	/// </summary>
	public enum TTTEnam
	{
		// 测试注释
		Aa,

	}
	/// <summary>
	///  
	/// </summary>
	delegate void AOnTestEvent<T>(T self) where T : INode;

	/// <summary>
	/// 测试接口
	/// </summary>
	public interface AITa
	{
		/// <summary>
		/// 
		/// </summary>
		public const int A_CT = 1;
	}
	/// <summary>
	/// a
	/// </summary>
	public struct AT
	{

		public const int aCC_BAA = 1;
		
		/// <summary>
		/// 测试变量
		/// </summary>
		private int a,v;

		public int ATa21;

	}

	/// <summary>
	/// 测试节点规则
	/// </summary>
	public static partial class DotNetTestNodeRule
	{
		/// <summary>
		/// 测试事件
		/// </summary>
		private static int A;

	}

	/// <summary>
	/// 测试节点
	/// </summary>
	public partial class DotNetTestNode : Node, ComponentOf<INode>
		, AsAwake
		, AsCurveEvaluate
		, AsTestEvent
	{
		/// <summary>
		/// act阿萨
		/// </summary>
		private int a;

		protected int cbacc1;

		private const int A_ACC_BAA = 1;

		/// <summary>
		/// a
		/// </summary>
		public List<int> NtList { get; }
		/// <summary>
		/// a
		/// </summary>
		public List<int> NtLi1LisList;

	

	
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