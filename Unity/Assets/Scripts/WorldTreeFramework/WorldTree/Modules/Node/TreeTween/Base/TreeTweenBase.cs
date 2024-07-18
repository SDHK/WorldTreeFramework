
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
		, AsTreeTaskTokenEvent
	{
		/// <summary>
		/// 启动标记
		/// </summary>
		[Protected] public bool isRun = false;

		/// <summary>
		/// 反向标记
		/// </summary>
		[Protected] public bool isReverse = false;

		/// <summary>
		/// 曲线
		/// </summary>
		[Protected] public CurveBase curve;

		/// <summary>
		/// 计时
		/// </summary>
		[Protected] public TimeSpan time;

		/// <summary>
		/// 定时
		/// </summary>
		[Protected] public TreeValueBase<float> clock;

		/// <summary>
		/// 时间尺度
		/// </summary>
		[Protected] public float timeScale;

		/// <summary>
		/// 完成回调
		/// </summary>
		public RuleActuator<ISendRule> OnCompleted;
	}
}
