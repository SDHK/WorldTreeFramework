
/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/5 19:46

* 描述： 对int值进行渐变的方法

*/

using System;

namespace WorldTree
{
    public static partial class TreeTweenRule
    {
        class IntTweenUpdateRule : TweenUpdateRule<TreeTween<int>>
        {
            protected override void Execute(TreeTween<int> self, TimeSpan deltaTime)
            {
                if (self.isRun)
                {
                    if (self.time.TotalSeconds < self.clock)
                    {
                        self.changeValue.Value = (int)((self.endValue.Value - self.startValue.Value) * self.GetCurveEvaluate(deltaTime) + self.startValue);
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
