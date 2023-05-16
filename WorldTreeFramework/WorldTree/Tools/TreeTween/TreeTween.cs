
/****************************************

* 作者： 闪电黑客
* 日期： 2022/12/14 15:34

* 描述： 树渐变节点

*/

using System;
using UnityEngine.UIElements;

namespace WorldTree
{

    /// <summary>
    /// 值渐变基类
    /// </summary>
    public class TreeTweenBase : Node
    {

        /// <summary>
        /// 启动标记
        /// </summary>
        public bool isRun = false;
        /// <summary>
        /// 曲线
        /// </summary>
        public CurveBase m_Curve;

        /// <summary>
        /// 曲线执行法则列表
        /// </summary>
        public IRuleList<ICurveEvaluateRule> m_RuleList;

        /// <summary>
        /// 完成回调
        /// </summary>
        public IRuleActuator<ITweenUpdateRule> OnCompleted;
        /// <summary>
        /// 计时
        /// </summary>
        public float time;

        /// <summary>
        /// 定时
        /// </summary>
        public float clock;

        /// <summary>
        /// 时间尺度
        /// </summary>
        public float timeScale;
    }

    /// <summary>
    /// 值渐变
    /// </summary>
    public class TreeTween<T1, T2> : TreeTweenBase, ComponentOf<TreeValueBase<T1>>
        , AsRule<IAwakeRule<T2, float>>
        , AsRule<ITweenUpdateRule>
        where T1 : IEquatable<T1>
        where T2 : IEquatable<T2>
    {
        /// <summary>
        /// 值
        /// </summary>
        public TreeValueBase<T1> changeValue;

        /// <summary>
        /// 开始
        /// </summary>
        public TreeValueBase<T1> startValue;

        /// <summary>
        /// 结束
        /// </summary>
        public TreeValueBase<T2> endValue;
    }


    public static class TreeTweenRule
    {
        /// <summary>
        /// 获取渐变
        /// </summary>
        public static TreeTween<T1, T2> GetTween<T1, T2>(this TreeValueBase<T1> changeValue, T2 endValue, float timeClock)
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
        {
            return changeValue.AddComponent(out TreeTween<T1, T2> _, endValue, timeClock);
        }

        /// <summary>
        /// 设置曲线 
        /// </summary>
        public static void SetCurve<C>(this TreeTweenBase self)
            where C : CurveBase
        {
            self.m_RuleList = self.GetRuleList<ICurveEvaluateRule>(typeof(C));
            self.m_Curve = self.Root.AddComponent(out CurveManager _).AddComponent(out C _);
        }

        /// <summary>
        /// 启动
        /// </summary>
        public static void Run(this TreeTweenBase self)
        {
            self.time = 0;
            self.isRun = true;
        }

        /// <summary>
        /// 曲线计算
        /// </summary>
        public static float GetCurveEvaluate(this TreeTweenBase self)
        {
            return self.m_RuleList.Call(self.m_Curve, self.timeScale, out float _);
        }
    }


    class TreeTweenAwakeTweenRule<T1, T2> : AwakeRule<TreeTween<T1, T2>, T2, float>
            where T1 : IEquatable<T1>
            where T2 : IEquatable<T2>
    {
        public override void OnEvent(TreeTween<T1, T2> self, T2 arg1, float arg2)
        {
            self.TryParentTo(out self.changeValue);
            self.startValue = self.AddChild(out TreeValue<T1> _);
            self.endValue = self.AddChild(out TreeValue<T2> _);

            self.startValue.Value = self.changeValue;
            self.endValue.Value = arg1;
            self.clock = arg2;
            self.time = 0;
        }
    }


    //匹配
    class TreeTweenTweenUpdateRule : TweenUpdateRule<TreeTween<float, float>>
    {
        public override void OnEvent(TreeTween<float, float> self, float deltaTime)
        {
            if (self.isRun)
            {
                if (self.time < self.clock)
                {
                    float vector = self.startValue.Value - self.endValue.Value;
                    self.time += deltaTime;
                    self.timeScale = self.time / self.clock;

                    self.timeScale = MathFloat.Clamp01(self.timeScale);
                    self.changeValue.Value = self.startValue + vector * self.GetCurveEvaluate();
                }
                else
                {
                    self.isRun = false;
                }
            }
        }
    }
}
