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
		public static async WorldTask WaitForCompletion(this TreeTweenBase self)
		{
			WorldTask asyncTask = self.World.PoolGetUnit<WorldTask>();
			WorldTaskToken token = await self.WorldTaskTokenCatch();
			Action<WorldTokenState> onToken = null;
			if (token != null)
			{
				onToken = state =>
				{
					switch (state)
					{
						case WorldTokenState.Running:
							self.isRun = true;
							break;
						case WorldTokenState.Stop:
							self.isRun = false;
							break;
						case WorldTokenState.Cancel:
							self.isRun = false;
							self.OnCompleted.Send();
							break;
					}
				};
				token.Add(onToken);
			}

			self.OnCompleted.Add(asyncTask, default(WorldTaskTimeout));
			await asyncTask;
			token?.Remove(onToken);
		}
	}
}
