/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

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
		public RuleMulticast<ISendRule> Callback;

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

        class AddRule : AddRule<TimerCycle>
        {
            protected override void Execute(TimerCycle self)
            {
                self.time = 0;
            }
        }
    }

}
