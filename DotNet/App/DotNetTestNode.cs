using System;

namespace WorldTree
{

	public partial class TestNode : Node, ComponentOf<DotNetTestNode>
		, AsAwake
	{
		public int TestValue;
	}

	/// <summary>
	/// 测试节点
	/// </summary>
	public partial class DotNetTestNode : Node, ComponentOf<INode>
		, AsAwake

	{
		public int TestValue;
	}



	public static partial class DotNetTestNodeRule
	{

		public static bool TryGetComponentBranch1<N, BN>(this N self, long key, out BN node)
			where N : class, INode, AsComponentBranch
			where BN : class, INode, NodeOf<N, ComponentBranch>
		=> (node = self.GetBranch<ComponentBranch>()?.GetNode(key) as BN) != null;



		/// <summary>
		/// 执行通知法则
		/// </summary>
		public static void SendRule1<N, R>(this N self, R nullRule)
			where N : class, AsCut
			where R : class, ISendRule
		{
			if (!self.Core.RuleManager.TryGetRuleList(self.Type, out IRuleList<R> ruleList)) return;
			ruleList.Send(self);
		}

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