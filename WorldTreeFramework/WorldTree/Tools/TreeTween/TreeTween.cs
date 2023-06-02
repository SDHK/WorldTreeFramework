
/****************************************

* 作者： 闪电黑客
* 日期： 2022/12/14 15:34

* 描述： 树渐变节点

*/

using System;

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
        /// 计时
        /// </summary>
        public float time;

        /// <summary>
        /// 定时
        /// </summary>
        public TreeValueBase<float> clock;

        /// <summary>
        /// 时间尺度
        /// </summary>
        public float timeScale;

        /// <summary>
        /// 完成回调
        /// </summary>
        public RuleActuator<ISendRuleBase> OnCompleted;
    }

    /// <summary>
    /// 值渐变
    /// </summary>
    public class TreeTween<T1> : TreeTweenBase, ComponentOf<TreeValueBase<T1>>
        , AsRule<IAwakeRule<T1, float>>
        , AsRule<ITweenUpdateRule>
        where T1 : IEquatable<T1>
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
        public TreeValueBase<T1> endValue;
    }


    public static class TreeTweenRule
    {
        /// <summary>
        /// 获取渐变
        /// </summary>
        public static TreeTween<T1> GetTween<T1>(this TreeValueBase<T1> changeValue, T1 endValue, float timeClock)
            where T1 : IEquatable<T1>
        {
            return changeValue.AddComponent(out TreeTween<T1> _, endValue, timeClock);
        }

        /// <summary>
        /// 设置曲线 
        /// </summary>
        public static TreeTweenBase SetCurve<C>(this TreeTweenBase self)
            where C : CurveBase
        {
            self.m_RuleList = self.GetRuleList<ICurveEvaluateRule>(typeof(C));
            self.m_Curve = self.Root.AddComponent(out CurveManager _).AddComponent(out C _);
            return self;
        }

        /// <summary>
        /// 启动
        /// </summary>
        public static TreeTweenBase Run(this TreeTweenBase self)
        {
            self.time = 0;
            self.isRun = true;
            return self;
        }

       

        /// <summary>
        /// 曲线计算
        /// </summary>
        public static float GetCurveEvaluate(this TreeTweenBase self)
        {
            return self.m_RuleList.Call(self.m_Curve, self.timeScale, out float _);
        }
    }




    class TreeTweenAwakeTweenRule<T1> : AwakeRule<TreeTween<T1>, T1, float>
            where T1 : IEquatable<T1>
    {
        public override void OnEvent(TreeTween<T1> self, T1 endValue, float clock)
        {
            self.TryParentTo(out self.changeValue);
            self.startValue = self.AddChild(out TreeValue<T1> _);
            self.endValue = self.AddChild(out TreeValue<T1> _);
            self.clock = self.AddChild(out TreeValue<float> _);

            self.AddChild(out self.OnCompleted);

            self.startValue.Value = self.changeValue;
            self.endValue.Value = endValue;
            self.clock.Value = clock;
            self.time = 0;
        }
    }

}
