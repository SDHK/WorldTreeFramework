
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/18 12:07

* 描述： 计数器组件，计数结束后回调

*/


namespace WorldTree
{
    /// <summary>
    /// 计数器
    /// </summary>
    public class CounterCall : Node, ComponentOf<INode>
    {
        public int count = 0;
        public int countOut = 0;
        //public Action callback;

        public IRuleActuator<ISendRule> callback;
    }

    public static class CounterCallRule
    {
        class AddRule : AddRule<CounterCall>
        {
            public override void OnEvent(CounterCall self)
            {
                self.count = 0;
                self.TryGetRuleActuator(out self.callback);
            }
        }

        class RemoveRule : RemoveRule<CounterCall>
        {
            public override void OnEvent(CounterCall self)
            {
                self.callback = null;
            }
        }


        class UpdateRule : UpdateRule<CounterCall>
        {
            public override void OnEvent(CounterCall self, float deltaTime)
            {
                self.count++;
                if (self.count >= self.countOut)
                {
                    self.callback?.Send();
                    self.Dispose();
                }
            }
        }
    }


}
