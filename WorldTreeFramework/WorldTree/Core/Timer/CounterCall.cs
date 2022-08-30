
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
    public class CounterCall : Entity
    {
        public int count = 0;
        public int countOut = 0;
        public Action callback;
    }

    class CounterCallGetSystem : GetSystem<CounterCall>
    {
        public override void OnGet(CounterCall self)
        {
            self.count = 0;
        }
    }

    class CounterCallUpdateSystem : UpdateSystem<CounterCall>
    {
        public override void Update(CounterCall self, float deltaTime)
        {
            self.count++;
            if (self.count >= self.countOut)
            {
                self.callback?.Invoke();
                self.RemoveSelf();
            }
        }
    }
}
