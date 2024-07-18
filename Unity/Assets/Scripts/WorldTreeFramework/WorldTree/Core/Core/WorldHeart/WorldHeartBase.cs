/****************************************

* 作者： 闪电黑客
* 日期： 2023/12/28 03:56:05

* 描述： 世界之心基类

*/

namespace WorldTree
{
	/// <summary>
	/// 世界之心基类
	/// </summary>
	public abstract class WorldHeartBase : Node, IListenerIgnorer
	{
		/// <summary>
		/// 是否运行
		/// </summary>
		public bool isRun;

		/// <summary>
		/// 单帧时间 (毫秒)
		/// </summary>
		[Protected]public int frameTime;

		/// <summary>
		/// 运行
		/// </summary>
		public abstract void Run();

		/// <summary>
		/// 暂停
		/// </summary>
		public abstract void Pause();

		/// <summary>
		/// 单帧运行
		/// </summary>
		public abstract void OneFrame();
	}
}
