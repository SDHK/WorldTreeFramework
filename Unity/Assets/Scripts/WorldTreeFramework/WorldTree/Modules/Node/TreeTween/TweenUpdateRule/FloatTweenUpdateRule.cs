
/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/17 20:12

* 描述： 对float值进行渐变的方法

*/

using System;

namespace WorldTree
{
    public static partial class TreeTweenRule
    {
        class FloatTweenUpdateRule : TweenUpdateRule<TreeTween<float>>
        {
            protected override void OnEvent(TreeTween<float> self, TimeSpan deltaTime)
            {
                if (self.isRun)
                {
                    if (self.time.TotalSeconds < self.clock)
                    {
                        self.changeValue.Value = (self.endValue.Value - self.startValue.Value) * self.GetCurveEvaluate(deltaTime) + self.startValue;
                    }
                    else
                    {
                        self.isRun = false;
                        self.OnCompleted.Send();
                    }
                }
            }
        }
    }
}
