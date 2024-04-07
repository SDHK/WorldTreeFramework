
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
        public float time = 0;
        public float timeOutTime = 0;
        public RuleActuator<ISendRuleBase> callback;

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
                        self.callback?.Send();
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
