
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


        class StringTweenUpdateRule : TweenUpdateRule<TreeTween<string>>
        {
            public override void OnEvent(TreeTween<string> self, float deltaTime)
            {
                if (self.isRun)
                {
                    if (self.time < self.clock)
                    {
                        //字符串的开始值和最终值的长度差值
                        int Length = (self.endValue.Value.Length - self.startValue.Value.Length);

                        //通过曲线获取长度变化
                        int nowLength = (int)(Length * self.GetCurveEvaluate(deltaTime) + self.startValue.Value.Length);

                        //当前长度不等于变化的长度时,进行拷贝赋值
                        if (self.changeValue.Value.Length != nowLength)
                        {
                            self.changeValue.Value = self.endValue.Value.Substring(0, nowLength);
                        }
                        //当前长度与结果相等，但是对象不同，直接进行引用赋值
                        else if (self.changeValue.Value.Length == self.endValue.Value.Length && self.endValue.Value != self.changeValue.Value)
                        {
                            self.changeValue.Value = self.endValue.Value;
                        }
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
