
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
    public class CounterCall : Node
    {
        public int count = 0;
        public int countOut = 0;
        public Action callback;
    }

    class CounterCallGetSystem : GetRule<CounterCall>
    {
        public override void OnEvent(CounterCall self)
        {
            self.count = 0;
        }
    }

    class CounterCallUpdateSystem : UpdateRule<CounterCall>
    {
        public override void OnEvent(CounterCall self, float deltaTime)
        {
            self.count++;
            if (self.count >= self.countOut)
            {
                self.callback?.Invoke();
                self.Dispose();
            }
        }
    }
}
