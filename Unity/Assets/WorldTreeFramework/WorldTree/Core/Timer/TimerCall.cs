
/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/17 9:56

* 描述： 计时器，计时结束后回调

*/

namespace WorldTree
{

    /// <summary>
    /// 计时器：单次调用
    /// </summary>
    public class TimerCall : Node, ComponentOf<INode>
        , AsRule<IAwakeRule<float>>
        , AsRule<ITreeTaskTokenEventRule>
    {
        public bool isRun = false;
        public float time = 0;
        public float timeOutTime = 0;

        /// <summary>
        /// 计时结束回调
        /// </summary>
        public RuleActuator<ISendRuleBase> callback;

        public override string ToString()
        {
            return $"TimerCall : {time} , {timeOutTime}";
        }
    }


    public static partial class TimerCallRule
    {
        class AwakeRule : AwakeRule<TimerCall, float>
        {
            protected override void OnEvent(TimerCall self, float timeOutTime)
            {
                self.timeOutTime = timeOutTime;
                self.time = 0;
                self.isRun = true;
                self.AddChild(out self.callback);
            }
        }

        class UpdateRule : UpdateTimeRule<TimerCall>
        {
            protected override void OnEvent(TimerCall self, float deltaTime)
            {
                if (self.IsActive && self.isRun)
                {
                    self.time += deltaTime;
                    if (self.time >= self.timeOutTime)
                    {
                        self.callback.Send();
                        self.Dispose();
                    }
                }

            }
        }

        class RemoveRule : RemoveRule<TimerCall>
        {
            protected override void OnEvent(TimerCall self)
            {
                self.isRun = false;
                self.callback = null;
            }
        }

        class TreeTaskTokenEventRule : TreeTaskTokenEventRule<TimerCall, TaskState>
        {
            protected override void OnEvent(TimerCall self, TaskState state)
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
