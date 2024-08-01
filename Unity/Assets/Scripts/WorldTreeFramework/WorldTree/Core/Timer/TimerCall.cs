/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/17 9:56

* 描述： 计时器，计时结束后回调

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 计时器：单次调用,不要挂在由它销毁的节点上
	/// </summary>
	public class TimerCall : Node, ComponentOf<INode>, TempOf<INode>
		, AsChildBranch
		, AsAwake<float>
		, AsTreeTaskTokenEvent
	{
		/// <summary>
		/// 是否运行
		/// </summary>
		public bool isRun = false;
		/// <summary>
		/// 计时
		/// </summary>
		public float time = 0;
		/// <summary>
		/// 计时结束时间
		/// </summary>
		public float timeOutTime = 0;

		/// <summary>
		/// 计时结束回调
		/// </summary>
		public RuleActuator<ISendRule> Callback;

		

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
				self.AddChild(out self.Callback);
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
						self.Callback?.Send();
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
				self.Callback = null;
			}
		}

		private class TreeTaskTokenEventRule : TreeTaskTokenEventRule<TimerCall>
		{
			protected override void Execute(TimerCall self, TokenState state)
			{
				switch (state)
				{
					case TokenState.Running:
						self.isRun = true;
						break;

					case TokenState.Stop:
						self.isRun = false;
						break;

					case TokenState.Cancel:
						self.isRun = false;
						self.Callback.Send();
						self.Dispose();
						break;
				}
			}
		}
	}
}