/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/17 9:56

* 描述： 计时器，计时结束后回调

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 计时器：单次调用
	/// </summary>
	public class TimerCall : Node, ComponentOf<INode>, TempOf<INode>
		, AsChildBranch
		, AsAwake<float>
		, AsTreeTaskTokenEvent
	{
		public bool isRun = false;
		public float time = 0;
		public float timeOutTime = 0;

		/// <summary>
		/// 计时结束回调
		/// </summary>
		public RuleActuator<ISendRule> callback;

		public override string ToString()
		{
			return $"TimerCall : {time} , {timeOutTime}";
		}
	}

	public static partial class TimerCallRule
	{
		private class AwakeRule : AwakeRule<TimerCall, float>
		{
			protected override void Execute(TimerCall self, float timeOutTime)
			{
				self.timeOutTime = timeOutTime;
				self.time = 0;
				self.isRun = true;
				self.AddChild(out self.callback);
			}
		}

		private class UpdateRule : UpdateTimeRule<TimerCall>
		{
			protected override void Execute(TimerCall self, TimeSpan deltaTime)
			{
				if (self.IsActive && self.isRun)
				{
					self.time += (float)deltaTime.TotalSeconds;
					if (self.time >= self.timeOutTime)
					{
						self.callback.Send();
						self.Dispose();
					}
				}
			}
		}

		private class RemoveRule : RemoveRule<TimerCall>
		{
			protected override void Execute(TimerCall self)
			{
				self.isRun = false;
				self.callback = null;
			}
		}

		private class TreeTaskTokenEventRule : TreeTaskTokenEventRule<TimerCall>
		{
			protected override void Execute(TimerCall self, TaskState state)
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