using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WorldTree
{
	public static partial class CascadeTickerTestRule
	{



		[NodeRule(nameof(UpdateRule<CascadeTickerTest>))]
		private static void OnUpdateRule(this CascadeTickerTest self)
		{
			if (Input.GetKeyDown(KeyCode.Q))
			{
				self.Log("添加定时器，3秒后触发");
				self.Core.RealTimeManager.AddTimerDelay<TestTickerCall>(TimeSpan.FromSeconds(3).Ticks, self);
			}
		}

		[NodeRule(nameof(TestTickerCallRule<CascadeTickerTest>))]
		private static void OnTestTickerCallRule(this CascadeTickerTest self)
		{
			self.Log("定时触发！！！");
		}

	
	}
}
