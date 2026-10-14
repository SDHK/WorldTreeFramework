/****************************************

* 作者：闪电黑客
* 日期：2026/10/14

* 描述：

*/
using System;

namespace WorldTree
{
	/// <summary>
	/// 切到目标世界线后再继续。任务对象不搬到目标树上。
	/// </summary>
	public class WorldTaskSwitchWorld : WorldTask
	{
		/// <summary>
		/// 目标世界线的线程上下文
		/// </summary>
		public WorldContext Target;

		/// <summary>
		/// 延续留在任务上，完成投递到目标世界线
		/// </summary>
		public override void UnsafeOnCompleted(Action continuation)
		{
			base.UnsafeOnCompleted(continuation);
			Target.Post(() =>
			{
				if (!IsCompleted) SetCompleted();
			});
		}

		public override void OnDispose()
		{
			Target = null;
			base.OnDispose();
		}
	}
}
