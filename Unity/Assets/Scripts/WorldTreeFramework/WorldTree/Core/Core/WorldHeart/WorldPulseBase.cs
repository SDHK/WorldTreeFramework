/****************************************

* 作者： 闪电黑客
* 日期： 2024/01/09 08:13:32

* 描述： 世界脉搏基类

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 世界脉搏基类
	/// </summary>
	public abstract class WorldPulseBase : Node
		, ComponentOf<WorldHeartBase>
		, AsRule<Awake<int>>
	{
		/// <summary>
		/// 是否运行
		/// </summary>
		public bool isRun;

		/// <summary>
		/// 单帧时间 (毫秒)
		/// </summary>
		[Protected] public int frameTime;

		/// <summary>
		/// 时间累计
		/// </summary>
		[Protected] public TimeSpan time = TimeSpan.Zero;

		/// <summary>
		/// 运行
		/// </summary>
		public abstract void Run();

		/// <summary>
		/// 单帧运行
		/// </summary>
		public abstract void OneFrame();

		/// <summary>
		/// 暂停
		/// </summary>
		public abstract void Pause();

		/// <summary>
		/// 刷新
		/// </summary>
		public abstract void Update(TimeSpan deltaTime);
	}
}
