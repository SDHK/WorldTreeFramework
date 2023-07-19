
/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/5 19:46

* 描述： 

*/

namespace WorldTree
{
    public static partial class TreeTweenRule
    {
        class IntTweenUpdateRule : TweenUpdateRule<TreeTween<int>>
        {
            public override void OnEvent(TreeTween<int> self, float deltaTime)
            {
                if (self.isRun)
                {
                    if (self.time < self.clock)
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
