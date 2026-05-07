/****************************************

* 作者：闪电黑客
* 日期：2024/6/17 10:23

* 描述：

*/
using System;

namespace WorldTree
{
	public static partial class TreeTweenRule
	{

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


	}
}
