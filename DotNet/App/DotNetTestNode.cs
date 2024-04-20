using System;

namespace WorldTree
{
	//public class TestNode001 : INode
	//{
		
	//}

	/// <summary>
	/// 测试节点
	/// </summary>
	public class DotNetTestNode : Node, ComponentOf<INode>
		, AsRule<IAwakeRule>
		, AsRule<IAwakeRule<float>>
	{
		public int TestValue;
	}


	//根据业务代码生成
	public static partial class DotNetTestNodeRule
	{
		private class DotNetTestNodeAwake : SendRuleBase<DotNetTestNode, IAwakeRule>
		{
			protected override void Execute(DotNetTestNode self)
			{
				self.Awake<IAwakeRule>();
			}
		}


		private class DotNetTestNodeAwake_float : SendRuleBase<DotNetTestNode, IAwakeRule<float>, float>
		{
			protected override void Execute(DotNetTestNode self, float value)
			{
				self.Awake<IAwakeRule<float>>(value);
			}
		}

		private class DotNetTestNodeUpdatet : SendRuleBase<DotNetTestNode, IUpdateRule>
		{
			protected override void Execute(DotNetTestNode self)
			{
				self.Update<IUpdateRule>();
			}
		}
	}

	//业务代码
	public static partial class DotNetTestNodeRule
	{
		private static void Awake<R>(this DotNetTestNode self)
			where R : IAwakeRule
		{

		}


		private static void Awake<R>(this DotNetTestNode self, float value)
			where R : IAwakeRule<float>
		{

		}

		private static void Update<R>(this DotNetTestNode self)
			where R : IUpdateRule
		{

		}
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