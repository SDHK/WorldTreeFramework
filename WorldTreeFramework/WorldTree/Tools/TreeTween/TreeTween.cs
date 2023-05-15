using System;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace WorldTree
{
    /// <summary>
    /// 曲线接口
    /// </summary>
    public interface ICurve
    {
        public float Evaluate(float x);
    }
    /// <summary>
    /// 曲线
    /// </summary>
    public class Curve : Node, ICurve, ComponentOf<TreeTweenBase>
    {
        public float Evaluate(float x)
        {
            return x;
        }
    }

    /// <summary>
    /// 值渐变基类
    /// </summary>
    public class TreeTweenBase : Node { }

    /// <summary>
    /// 值渐变
    /// </summary>
    public class TreeTween<T1, T2> : TreeTweenBase, ComponentOf<TreeValueBase<T1>>
        where T1 : IEquatable<T1>
        where T2 : IEquatable<T2>
    {
        /// <summary>
        /// 自动刷新标记 :判断结束与值来启动
        /// </summary>
        public bool isAuto;//??

        /// <summary>
        /// 启动标记
        /// </summary>
        public bool isRun;


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

        /// <summary>
        /// 曲线
        /// </summary>
        public ICurve curve;

        /// <summary>
        /// 完成回调
        /// </summary>
        public IRuleActuator<ITweenRule> OnCompleted;

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


    class TreeTweenUpdateRule : UpdateRule<TreeTween<float, float>>
    {
        public override void OnEvent(TreeTween<float, float> self, float timeDelta)
        {
            if (self.time < self.clock && self.isRun)
            {
                float vector = self.startValue.Value - self.endValue.Value;
                self.time += timeDelta;
                self.timeScale = self.time / self.clock;
                self.changeValue.Value = self.startValue + vector * self.curve.Evaluate(self.timeScale);
            }
        }
    }




    public static class TreeTweenRule
    {


    }



}
