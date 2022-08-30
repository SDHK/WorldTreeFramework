
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
    public class TimerCycle : Entity
    {
        public float time = 0;
        public float timeOutTime = 0;
        public Action callback;
    }

    class TimerCycleUpdateSystem : UpdateSystem<TimerCycle>
    {
        public override void Update(TimerCycle self, float deltaTime)
        {
            self.time += deltaTime * self.Domain.AddComponent<TimeDomain>().timeScale;
            if (self.time >= self.timeOutTime)
            {
                self.time = 0;
                self.callback?.Invoke();
            }
        }
    }

    class TimerCycleGetSystem : GetSystem<TimerCycle>
    {
        public override void OnGet(TimerCycle self)
        {
            self.time = 0;
        }
    }
}
