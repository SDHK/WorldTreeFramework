using System;

namespace WorldTree
{
	/// <summary>
	/// 测试节点
	/// </summary>
	class DotNetTestNode : Node, ComponentOf<INode>
		,AsRule<IAwakeRule>
	{

	}

	public static partial class DotNetTestNodeRule
	{
		class AddRule : AddRule<DotNetTestNode>
		{
			protected override void OnEvent(DotNetTestNode self)
			{
				self.Log(" 初始化！！！");

				//self.Log(self.Core.ToStringDrawTree());
			}
		}

		class UpdateTimeRule : UpdateTimeRule<DotNetTestNode>
		{
			protected override void OnEvent(DotNetTestNode self, TimeSpan timeSpan)
			{
				self.Log($"初始更新！！{timeSpan.TotalSeconds}");
			}
		}

		class RemoveRule : RemoveRule<DotNetTestNode>
		{
			protected override void OnEvent(DotNetTestNode self)
			{
				self.Log($"初始关闭！！");
			}
		}

	}



}
