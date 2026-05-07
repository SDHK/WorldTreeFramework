using UnityEngine;

namespace WorldTree
{
	/// <summary>
	/// 测试定时器规则 
	/// </summary>
	public interface TestTickerCall : ISendRule { }

	/// <summary>
	/// 级联定时器测试 
	/// </summary>
	public partial class CascadeTickerTest : Node
		, ComponentOf<InitialDomain>
		, AsRule<Awake>
		, AsComponentBranch
		, AsChildBranch
		, AsRule<TestTickerCall>
	{

		[NodeRule(nameof(UpdateRule<CascadeTickerTest>))]
		private static void OnUpdateRule(CascadeTickerTest self)
		{
			if (Input.GetKeyDown(KeyCode.Q))
			{
				self.Log("添加定时器，3秒后触发");
				//self.Core.RealTimeManager.AddTimerDelay<TestTickerCall>(TimeSpan.FromSeconds(3).Ticks, self);
			}
		}

		[NodeRule(nameof(TestTickerCallRule<CascadeTickerTest>))]
		private static void OnTestTickerCallRule(CascadeTickerTest self)
		{
			self.Log("定时触发！！！");
		}
	}

}
