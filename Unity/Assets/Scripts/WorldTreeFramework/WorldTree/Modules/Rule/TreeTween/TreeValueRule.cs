/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
using System;

namespace WorldTree
{
	public static partial class TreeValueRule
	{

		/// <summary>
		/// 做渐变
		/// </summary>
		public static TreeTween<T> DoTween<T>(this TreeValueBase<T> self, T endValue, float time)
			where T : IEquatable<T>
		{
			TreeTween<T> treeTween = self.GetTween(endValue, time);
			treeTween.Run();
			return treeTween;
		}


		/// <summary>
		/// 等待完成
		/// </summary>
		public static async TreeTask WaitForCompletion(this TreeTweenBase self)
		{
			TreeTask asyncTask = self.AddTemp(out TreeTask _);

			//令牌是否为空,不为空则将组件挂入令牌
			if (await self.TreeTaskTokenCatch() is TreeTaskToken taskToken)
			{
				taskToken.TokenEvent.Add(self);
			}

			//组件的任务完成回调注册
			self.OnCompleted.Add(asyncTask, default(TreeTaskSetResuIt));

			//等待异步执行
			await asyncTask;
		}
	}
}
