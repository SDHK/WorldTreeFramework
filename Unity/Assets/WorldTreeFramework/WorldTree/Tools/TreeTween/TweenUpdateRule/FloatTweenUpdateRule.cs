
/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/17 20:12

* 描述： 

*/

namespace WorldTree
{
    public static partial class TreeTweenRule
    {
        class FloatTweenUpdateRule : TweenUpdateRule<TreeTween<float>>
        {
            public override void OnEvent(TreeTween<float> self, float deltaTime)
            {
                if (self.isRun)
                {
                    if (self.time < self.clock)
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
