/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/18 12:07

* 描述： 计数器组件，计数结束后回调

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 计数器
	/// </summary>
	public class CounterCall : Node, ComponentOf<INode>
		, AsRule<IAwakeRule<int>>
		, AsRule<TreeTaskTokenEvent>

	{
		public bool isRun = false;
		public int count = 0;
		public int countOut = 0;

		/// <summary>
		/// 计数结束回调
		/// </summary>
		public RuleActuator<ISendRuleBase> callback;

		public override string ToString()
		{
			return $"CounterCall : {count} , {countOut}";
		}
	}

	public static partial class CounterCallRule
	{
		private class AwakeRule : AwakeRule<CounterCall, int>
		{
			protected override void Execute(CounterCall self, int count)
			{
				self.count = count;
				self.countOut = count;
				self.isRun = true;
				self.AddChild(out self.callback);
			}
		}

		private class UpdateRule : UpdateRule<CounterCall>
		{
			protected override void Execute(CounterCall self)
			{
				if (self.IsActive && self.isRun)
				{
					self.count++;
					if (self.count >= self.countOut)
					{
						self.callback.Send();
						self.Dispose();
					}
				}
			}
		}

		private class RemoveRule : RemoveRule<CounterCall>
		{
			protected override void Execute(CounterCall self)
			{
				self.isRun = false;
				self.callback = null;
			}
		}

		private class TreeTaskTokenEventRule : TreeTaskTokenEventRule<CounterCall>
		{
			protected override void Execute(CounterCall self, TaskState state)
			{
				switch (state)
				{
					case TaskState.Running:
						self.isRun = true;
						break;

					case TaskState.Stop:
						self.isRun = false;
						break;

					case TaskState.Cancel:
						self.Dispose();
						break;
				}
			}
		}
	}
}