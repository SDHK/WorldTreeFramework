
/****************************************

* 作者： 闪电黑客
* 日期： 2023/5/17 20:12

* 描述： 

*/

namespace WorldTree
{
    class FloatTweenUpdateRule : TweenUpdateRule<TreeTween<float>>
    {
        public override void OnEvent(TreeTween<float> self, float deltaTime)
        {
            if (self.isRun)
            {
                if (self.time < self.clock)
                {
                    float vector = self.endValue.Value - self.startValue.Value;
                    self.time += deltaTime;
                    self.timeScale = self.time / self.clock;

                    self.timeScale = MathFloat.Clamp01(self.timeScale);
                    self.changeValue.Value = self.startValue + vector * self.GetCurveEvaluate();
                    //World.Log($"时间 {self.timeScale} 渐变： {self.changeValue.Value}");
                }
                else
                {
                    self.isRun = false;
                    self.OnCompleted.Send();
                    //World.Log("渐变结束：" + self.changeValue.Value);
                }
            }
        }
    }
}
