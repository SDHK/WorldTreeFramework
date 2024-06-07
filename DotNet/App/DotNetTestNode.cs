using System;

namespace WorldTree
{
	public interface CallTast<T1, T2> : ICallRule<T1, T2,float> { }

	public interface SendTast<T1, T2> : ISendRule<T1, T2> { }

	//public static partial class DotNetTestNodeRule
	//{
	//	class OnCallTastRuleExecute : CallTastRule<DotNetTestNode, int, int>
	//	{
	//		protected override int Execute(DotNetTestNode self, int arg1)
	//		=> OnCallTast(self, arg1);
	//	}
	//}


	/// <summary>
	/// 测试节点
	/// </summary>
	public partial class DotNetTestNode : Node, ComponentOf<INode>
		, AsAwake
		, AsCallTast<int, int>
		, AsCurveEvaluate
		, AsSendTast<int, int>
	{
		public int TestValue;
	}

	public static partial class T1
	{
		private static int a;
	}

	public static partial class DotNetTestNodeRule
	{
		private static OnCallTast<DotNetTestNode, int, int> OnCallTast =
		(self, t1,t2) =>
		{
			return 123f;
		};

		private static OnCurveEvaluate<DotNetTestNode> OnCurveEvaluate =
		(self, t1) =>
		{
			return 1;
		};

		static OnUpdateTime<DotNetTestNode> updateTime =
	   (self, timeSpan) =>
	   {
		   self.Log($"初始更新！！！{timeSpan.TotalSeconds}");
	   };



		private static OnUpdate<DotNetTestNode> OnUpdat123 =
		(self) =>
		{

		};

		private static OnSendTast<DotNetTestNode, int, int> OnSendTast =
		 delegate (DotNetTestNode self, int t1, int t2)
		{

		};





		private class New : NewRule<DotNetTestNode>
		{
			protected override void Execute(DotNetTestNode self)
			{
				self.Log("新建！！");

			}
		}


		private class EnableRule : EnableRule<DotNetTestNode>
		{
			protected override void Execute(DotNetTestNode self)
			{
				self.Log("激活！！");
			}
		}

		private class AddRule : AddRule<DotNetTestNode>
		{
			protected override void Execute(DotNetTestNode self)
			{
				self.Log(" 初始化！！！");
			}
		}

		private class DisableRule : DisableRule<DotNetTestNode>
		{
			protected override void Execute(DotNetTestNode self)
			{
				self.Log("失活！！");
			}
		}

		private class UpdateTimeRule : UpdateTimeRule<DotNetTestNode>
		{
			protected override void Execute(DotNetTestNode self, TimeSpan timeSpan)
			{
				self.Log($"初始更新！！！{timeSpan.TotalSeconds}");
			}
		}

		private class RemoveRule : RemoveRule<DotNetTestNode>
		{
			protected override void Execute(DotNetTestNode self)
			{
				self.Log($"初始关闭！！");
			}
		}
	}

}