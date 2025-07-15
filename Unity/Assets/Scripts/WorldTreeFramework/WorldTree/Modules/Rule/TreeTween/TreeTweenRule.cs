﻿/****************************************

* 作者：闪电黑客
* 日期：2024/6/17 10:23

* 描述：

*/
using System;

namespace WorldTree
{
	public static partial class TreeTweenRule
	{
		class TreeTaskTokenEventRule : TreeTaskTokenEventRule<TreeTweenBase>
		{
			protected override void Execute(TreeTweenBase self, TokenState state)
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

			self.GetParent(out self.changeValue);
			self.startValue.Value = self.changeValue;
			self.endValue.Value = endValue;
			self.clock.Value = clock;
			self.time = TimeSpan.Zero;
			return self;
		}

		/// <summary>
		/// 设置曲线 
		/// </summary>
		public static TreeTweenBase SetCurve<C>(this TreeTweenBase self)
			where C : CurveBase
		{
			self.curve = self.World.AddComponent(out CurveManager _).AddComponent(out C _);
			return self;
		}

		/// <summary>
		/// 启动
		/// </summary>
		public static TreeTweenBase Run(this TreeTweenBase self)
		{
			if (self.curve == null) self.World.AddComponent(out CurveManager _).AddComponent(out self.curve);
			self.time = TimeSpan.Zero;
			self.isRun = true;
			self.isReverse = false;
			return self;
		}


		/// <summary>
		/// 曲线计算
		/// </summary>
		public static float GetCurveEvaluate(this TreeTweenBase self, TimeSpan deltaTime)
		{
			return self.curve.CurveEvaluate(self.GetTimeScale(deltaTime));
		}

		/// <summary>
		/// 时间尺度计算
		/// </summary>
		public static float GetTimeScale(this TreeTweenBase self, TimeSpan deltaTime)
		{

			self.time += deltaTime * (self.isReverse ? -1 : 1);

			if (self.isReverse && self.time < TimeSpan.Zero)
			{
				self.time = TimeSpan.Zero;
				self.isRun = false;
			}

			self.timeScale = (float)self.time.TotalSeconds / self.clock;
			self.timeScale = MathFloat.Clamp01(self.timeScale);


			return self.timeScale;
		}
	}
}
