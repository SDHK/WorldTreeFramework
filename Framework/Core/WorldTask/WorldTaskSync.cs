/****************************************

* 作者：闪电黑客
* 日期：2026/10/14

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 已完成的同步任务。用来让本来就能返回的异步方法有一次 await。
	/// </summary>
	public class WorldTaskCompleted : WorldTask, IWorldSyncTask
	{
	}

	/// <summary>
	/// 读取当前任务链上的令牌
	/// </summary>
	public class WorldTaskTokenCatch : WorldTask<WorldTaskToken>, IWorldSyncTask
	{
		/// <summary>
		/// 获取令牌
		/// </summary>
		public override WorldTaskToken GetResult()
		{
			Result = Token;
			return base.GetResult();
		}
	}
}
