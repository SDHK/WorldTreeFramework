/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// 树渐变基类
	/// </summary>
	public partial class TreeTweenBase : Node
		, AsRule<TreeTaskTokenEvent>
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
		public RuleMulticast<ISendRule> OnCompleted;

		[NodeRule(nameof(TreeTaskTokenEventRule<TreeTweenBase>))]
		private static void OnTreeTaskTokenEventRule(TreeTweenBase self, TokenState state)
		{
			switch (state)
			{
				case TokenState.Running:
					self.isRun = true;
					break;
				case TokenState.Stop:
					self.isRun = false;
					break;
				case TokenState.Cancel:
					self.isRun = false;
					self.OnCompleted.Send();
					break;
			}
		}


		/// <summary>
		/// 设置曲线 
		/// </summary>
		public TreeTweenBase SetCurve<C>()
			where C : CurveBase
		{
			this.curve = this.World.AddComponent(out CurveManager _).AddComponent(out C _);
			return this;
		}

		/// <summary>
		/// 启动
		/// </summary>
		public TreeTweenBase Run()
		{
			if (this.curve == null) this.World.AddComponent(out CurveManager _).AddComponent(out this.curve);
			this.time = TimeSpan.Zero;
			this.isRun = true;
			this.isReverse = false;
			return this;
		}


		/// <summary>
		/// 曲线计算
		/// </summary>
		public float GetCurveEvaluate(TimeSpan deltaTime)
		{
			return this.curve.CurveEvaluate(this.GetTimeScale(deltaTime));
		}

		/// <summary>
		/// 时间尺度计算
		/// </summary>
		public float GetTimeScale(TimeSpan deltaTime)
		{

			this.time += deltaTime * (this.isReverse ? -1 : 1);

			if (this.isReverse && this.time < TimeSpan.Zero)
			{
				this.time = TimeSpan.Zero;
				this.isRun = false;
			}

			this.timeScale = (float)this.time.TotalSeconds / this.clock;
			this.timeScale = MathFloat.Clamp01(this.timeScale);


			return this.timeScale;
		}


	}
}
