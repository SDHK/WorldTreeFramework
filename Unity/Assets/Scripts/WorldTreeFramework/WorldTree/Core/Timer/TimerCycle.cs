
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/18 11:51

* 描述： 计时器：循环调用

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 计时器：循环调用
    /// </summary>
    public class TimerCycle : Node
    {
		/// <summary>
		/// 是否运行
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

    }


    public static partial class TimerCycleRule
    {
        class UpdateRule : UpdateTimeRule<TimerCycle>
        {
            protected override void Execute(TimerCycle self, TimeSpan deltaTime)
            {
                if (self.IsActive)
                {
                    self.time += (float)deltaTime.TotalSeconds;
                    if (self.time >= self.timeOutTime)
                    {
                        self.time = 0;
                        self.Callback?.Send();
                    }
                }
            }
        }

        class GetRule : GetRule<TimerCycle>
        {
            protected override void Execute(TimerCycle self)
            {
                self.time = 0;
            }
        }
    }

}
