
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
                        //选取最长的值
                        var longValue = self.startValue.Value.Length > self.endValue.Value.Length ? self.startValue.Value : self.endValue.Value;
                        var shortValue = self.startValue.Value.Length > self.endValue.Value.Length ? self.endValue.Value : self.startValue.Value;


                        //判断长包含短，否则清空开始值
                        if (self.startValue.Value.Length != 0 && (!longValue.StartsWith(shortValue) && longValue != string.Empty))
                        {
                            self.startValue.Value = string.Empty;
                        }


                        //字符串的开始值和最终值的长度差值
                        int Length = (self.endValue.Value.Length - self.startValue.Value.Length);

                        //通过曲线获取长度变化
                        int nowLength = (int)(Length * self.GetCurveEvaluate(deltaTime) + self.startValue.Value.Length);


                        //当前长度不等于变化的长度时，进行拷贝赋值
                        if (self.changeValue.Value.Length != nowLength)
                        {
                            self.changeValue.Value = longValue.Substring(0, nowLength);
                        }
                        //当前长度 相等 结果长度，但是对象不同，直接进行引用赋值
                        else if (self.changeValue.Value.Length == self.endValue.Value.Length && self.endValue.Value != self.changeValue.Value)
                        {
                            self.changeValue.Value = self.endValue.Value;
                        }
                        //当前长度 等于 当前裁剪长度时，但内容不同，进行拷贝赋值
                        else if (!self.endValue.Value.StartsWith(self.changeValue.Value))
                        {
                            self.changeValue.Value = longValue.Substring(0, nowLength);
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
