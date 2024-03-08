
/****************************************

* 作者： 闪电黑客
* 日期： 2022/12/14 15:34

* 描述： 树渐变基类

*/

using System;

namespace WorldTree
{
    /// <summary>
    /// 树渐变基类
    /// </summary>
    public class TreeTweenBase : Node
        , AsRule<ITreeTaskTokenEventRule>
    {
        /// <summary>
        /// 启动标记
        /// </summary>
        public bool isRun = false;

        /// <summary>
        /// 反向标记
        /// </summary>
        public bool isReverse = false;

        /// <summary>
        /// 曲线
        /// </summary>
        public CurveBase m_Curve;

        /// <summary>
        /// 计时
        /// </summary>
        public TimeSpan time;

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
}
