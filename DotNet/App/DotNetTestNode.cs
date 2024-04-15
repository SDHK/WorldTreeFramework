using System;

namespace WorldTree
{
	/// <summary>
	/// 测试节点
	/// </summary>
	public class DotNetTestNode : Node, ComponentOf<INode>
		, AsRule<IAwakeRule>
	{
		public int TestValue;
	}

	public static partial class DotNetTestNodeRule
	{
		private class AwakeRule : AwakeRule<DotNetTestNode>
		{
			protected override void Execute(DotNetTestNode self)
			{
				self.Log("唤醒！！");
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