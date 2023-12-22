using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

				self.Log(self.Core.ToStringDrawTree());
			}
		}

	}



}
