
/****************************************

* 作者： 闪电黑客
* 日期： 2023/7/10 17:26

* 描述： 

*/

using System;

namespace WorldTree
{
    public static partial class TreeTweenRule
    {
        class TreeTaskTokenEventRule : TreeTaskTokenEventRule<TreeTweenBase, TaskState>
        {
            protected override void OnEvent(TreeTweenBase self, TaskState state)
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
                        self.isRun = true;
                        self.isReverse = true;
                        break;
                }
            }
        }


        /// <summary>
        /// 获取渐变
        /// </summary>
        public static TreeTween<T1> GetTween<T1>(this TreeValueBase<T1> changeValue, T1 endValue, float timeClock)
            where T1 : IEquatable<T1>
        {
            return changeValue.AddComponent(out TreeTween<T1> _).Set(endValue, timeClock);
        }

        /// <summary>
        /// 设置
        /// </summary>
        public static TreeTween<T1> Set<T1>(this TreeTween<T1> self, T1 endValue, float clock)
           where T1 : IEquatable<T1>
        {
            self.startValue ??= self.AddChild(out TreeValue<T1> _);
            self.endValue ??= self.AddChild(out TreeValue<T1> _);
            self.clock ??= self.AddChild(out TreeValue<float> _);
            self.OnCompleted ??= self.AddChild(out self.OnCompleted);

            self.TryParentTo(out self.changeValue);
            self.startValue.Value = self.changeValue;
            self.endValue.Value = endValue;
            self.clock.Value = clock;
            self.time = 0;
            return self;
        }

        /// <summary>
        /// 设置曲线 
        /// </summary>
        public static TreeTweenBase SetCurve<C>(this TreeTweenBase self)
            where C : CurveBase, AsRule<IAwakeRule>
        {
            self.m_Curve = self.Root.AddComponent(out CurveManager _).AddComponent(out C _);
            return self;
        }

        /// <summary>
        /// 启动
        /// </summary>
        public static TreeTweenBase Run(this TreeTweenBase self)
        {
            if (self.m_Curve == null) self.Root.AddComponent(out CurveManager _).AddComponent(out self.m_Curve);
            self.time = 0;
            self.isRun = true;
            self.isReverse = false;
            return self;
        }


        /// <summary>
        /// 曲线计算
        /// </summary>
        public static float GetCurveEvaluate(this TreeTweenBase self, float deltaTime)
        {
            return self.m_Curve.CallRule(NullType<ICurveEvaluateRule>.Null, self.GetTimeScale(deltaTime), out float _);
        }

        /// <summary>
        /// 时间尺度计算
        /// </summary>
        public static float GetTimeScale(this TreeTweenBase self, float deltaTime)
        {

            self.time += deltaTime * (self.isReverse ? -1 : 1);

            if (self.isReverse && self.time < 0)
            {
                self.time = 0;
                self.isRun = false;
            }

            self.timeScale = self.time / self.clock;
            self.timeScale = MathFloat.Clamp01(self.timeScale);


            return self.timeScale;
        }
    }
}
