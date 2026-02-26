using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldTree
{
	/// <summary>
	/// 测试定时器规则 
	/// </summary>
	public interface TestTickerCall : ISendRule { }

	/// <summary>
	/// 级联定时器测试 
	/// </summary>
	public class CascadeTickerTest : Node
		, ComponentOf<InitialDomain>
		, AsRule<Awake>
		, AsComponentBranch
		, AsChildBranch
		, AsRule<TestTickerCall>
	{
	}

}
