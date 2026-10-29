using System;
using WorldTree;

namespace TestProject
{
	public class CascadeTickerChangeTickTest
	{
		private class TestWorld : World { }

		private class TestHeart : WorldHeartBase
		{
			public override void Run() { }
			public override void Pause() { }
			public override void OneFrame() { }
		}

		[Fact]
		public void ChangeTickOnlyAllowsLater()
		{
			var core = new WorldTreeCore();
			core.SetOptions(new Options());
			var line = core.Create(0, typeof(TestWorld), typeof(TestHeart), 0);

			var ticker = line.WorldTimeManager.Timer;
			ticker.AddChild(out CascadeTickerData data, 100L, (INode)line, default(RuleList)!);

			ticker.ChangeTick(data.Id, 200);
			Assert.Equal(200, data.Tick);

			Assert.Throws<ArgumentOutOfRangeException>(() => ticker.ChangeTick(data.Id, 150));
			Assert.Throws<Exception>(() => ticker.ChangeTick(0, 300));

			line.Dispose();
		}
	}
}
