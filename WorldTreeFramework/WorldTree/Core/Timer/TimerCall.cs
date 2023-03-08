
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
    public class TimerCall : Node
    {
        public float time = 0;
        public float timeOutTime = 0;
        public Action callback;
    }

    class TimerCallUpdateRule : UpdateRule<TimerCall>
    {
        public override void OnEvent(TimerCall self, float deltaTime)
        {
            self.time += deltaTime;
            if (self.time >= self.timeOutTime)
            {
                self.callback?.Invoke();
                self.Dispose();
            }
        }
    }

    class TimerCallGetRule : GetRule<TimerCall>
    {
        public override void OnEvent(TimerCall self)
        {
            self.time = 0;
        }
    }
}
