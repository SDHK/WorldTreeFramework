using System;

namespace WorldTree
{
	public interface TestEvent : ISendRule, IMethodRule{}

	public class TestEventINode : SendRuleDefault<TestEvent>
	{
		protected override void Execute(INode self)
		{
			self.Log($"法则默认实现");
		}
	}


	/// <summary>
	/// 测试节点
	/// </summary>
	public partial class DotNetTestNode : Node, ComponentOf<INode>
		, AsAwake
		, AsAwake<string>
		, AsNew
		, AsTestEvent
	{
		public int TestValue;
	}

	public static partial class DotNetTestNodeRule
	{
		private class New : NewRule<DotNetTestNode>
		{
			protected override void Execute(DotNetTestNode self)
			{
				self.Log("新建！！");
				self.TestEvent();//调用
			}
		}

		public class TestEvent : TestEventRule<DotNetTestNode>
		{
			protected override void Execute(DotNetTestNode self)
			{
				self.Log($"TestEventRule子类重载实现");
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