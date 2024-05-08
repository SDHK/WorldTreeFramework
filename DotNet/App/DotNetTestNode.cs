using System;
using System.Numerics;

namespace WorldTree
{
	public interface ITestRule : ISendRuleBase
	{ }

	public interface ITestRule<T1> : ISendRuleBase<T1>
		where T1 : IEquatable<T1>
	{ }

	public interface ITestRuleFloat<T1, T2> : ISendRuleBase<T1, Vector3, T2>
		where T1 : IEquatable<T1>
		where T2 : class
	{ }

	public interface ITestRuleFloat2 : ISendRuleBase<float>
	{ }

	/// <summary>
	/// 测试节点
	/// </summary>
	public partial class DotNetTestNode : Node, ComponentOf<INode>
		, AsAwakeRule
		, AsAwakeRule<string>

	{
		public int TestValue;
	}

	public static partial class DotNetTestNodeRule
	{
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
				self.Log(" 初始化！！！!!");

				//self.AddComponent(out RefeshCsProjFileCompileInclude _);
				//self.Log(self.Core.ToStringDrawTree());
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
				self.Log($"初始更新！！{timeSpan.TotalSeconds}");
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