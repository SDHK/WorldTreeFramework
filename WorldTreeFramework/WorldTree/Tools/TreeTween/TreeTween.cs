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
    public class TreeTween<T> : Node, ComponentOf<TreeValueBase>
        , AsRule<IAwakeRule>
        , AsRule<IUpdateRule>
        , AsRule<ITweenRule<T>>
        where T : IEquatable<T>
    {
        /// <summary>
        /// 自动刷新标记 :判断结束与值来启动
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
        public IRuleActuator<ISendRule> OnCompleted;

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

    class TreeTweenUpdateRule<T> : UpdateRule<TreeTween<T>>
        where T : IEquatable<T>
    {
        public override void OnEvent(TreeTween<T> self, float arg1)
        {

        }
    }



    public static class TreeTweenRule
    {


    }



}
