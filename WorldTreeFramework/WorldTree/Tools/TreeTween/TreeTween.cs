using System;
using Unity.Mathematics;

namespace WorldTree
{

    //public class FloatTweenRule : TweenRule<float>
    //{
    //    public Func<float, float> Curve;
    //    public override void OnEvent(TreeTween<float> self)
    //    {
    //        float vector = self.startValue.Value - self.endValue.Value;


    //        //self.changeValue.Value = 
    //    }
    //}

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
    public class Curve : Node, ICurve
    {
        public float Evaluate(float x)
        {
            return x;
        }
    }



    /// <summary>
    /// 值渐变
    /// </summary>
    public class TreeTween<T> : Node, IAwake, ComponentOf<TreeValueBase>
        where T : IEquatable<T>
    {
        /// <summary>
        /// 自动刷新标记
        /// </summary>
        public bool isAuto;

        /// <summary>
        /// 启动标记
        /// </summary>
        public bool isRun;


        /// <summary>
        /// 值
        /// </summary>
        public TreeValueBase<T> changeValue;

        /// <summary>
        /// 开始
        /// </summary>
        public TreeValueBase<T> startValue;

        /// <summary>
        /// 结束
        /// </summary>
        public TreeValueBase<T> endValue;

        //判断结束与值来启动


        public float time;

        public float timeScale;

        public float timeDelta;

        /// <summary>
        /// 执行法则列表
        /// </summary>
        public IRuleList<ITweenRule<T>> ruleList;

        /// <summary>
        /// 曲线
        /// </summary>
        public ICurve curve;


        /// <summary>
        /// 完成回调
        /// </summary>
        public RuleActuator OnComplete;



    }



    public static class TreeTweenRule
    {


    }



}
