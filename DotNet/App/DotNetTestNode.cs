using System;

namespace WorldTree
{
	/// <summary>
	/// 测试节点
	/// </summary>
	public class DotNetTestNode : Node, ComponentOf<INode>
		, AsAwakeRule
		, AsAwakeRule<string>
	{
		public int TestValue;
	}

	public static partial class DotNetTestNodeRule
	{
		//代码生成
		public static void AwakeRule(this AsAwakeRule awakeRule)
		{
			awakeRule.SendRule(TypeInfo<IAwakeRule>.Default);
		}

		//代码生成
		public static void AwakeRule1<T1>(this AsAwakeRule<T1> awakeRule, T1 t1)
		{
			awakeRule.SendRule(TypeInfo<IAwakeRule<T1>>.Default, t1);
		}

		//代码生成
		public static void AwakeRule2<T1, T2>(this AsRule<IAwakeRule<T1, T2>> awakeRule, T1 t1, T2 t2)
		{
		}

		private class EnableRule : EnableRule<DotNetTestNode>
		{
			protected override void Execute(DotNetTestNode self)
			{
				//尝试调用
				self.AwakeRule();

				//尝试调用
				self.AwakeRule1("1f");

				//有提示参数不对
				self.AwakeRule1(1f);

				//没有就是直接白色没有提示
				self.AwakeRule2("2", "1f");

				//原来的代码，就算没有也有提示
				self.SendRule(TypeInfo<IAwakeRule<long, int>>.Default, 1, 1);

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