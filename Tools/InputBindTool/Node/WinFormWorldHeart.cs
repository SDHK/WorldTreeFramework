namespace WorldTree
{
	/// <summary>
	/// 世界之心：WinForm线程
	/// </summary>
	public class WinFormWorldHeart : WorldHeartBase
		, AsComponentBranch
		, CoreManagerOf<WorldLine>
		, AsAwake<int>
	{
		/// <summary>
		/// 上一次运行时间
		/// </summary>
		public DateTime afterTime;

		#region 世界脉搏
		/// <summary>
		/// 世界脉搏 UpdateTime
		/// </summary>
		public WorldPulse<UpdateTime> worldUpdate;

		#endregion

		#region 全局事件法则

		/// <summary>
		/// 全局事件法则 Enable
		/// </summary>
		public IRuleExecutor<Enable> enable;
		/// <summary>
		/// 全局事件法则 Disable
		/// </summary>
		public IRuleExecutor<Disable> disable;
		/// <summary>
		/// 全局事件法则 Update
		/// </summary>
		public IRuleExecutor<Update> update;
		/// <summary>
		/// 全局事件法则 UpdateTime
		/// </summary>
		public IRuleExecutor<UpdateTime> updateTime;

		#endregion

		/// <summary>
		/// 运行
		/// </summary>
		public override void Run()
		{
			isRun = true;
		}

		/// <summary>
		/// 暂停
		/// </summary>
		public override void Pause()
		{
			isRun = false;
		}

		/// <summary>
		/// 单帧运行
		/// </summary>
		public override void OneFrame()
		{
			isRun = false;
			worldUpdate?.Update((DateTime.Now - afterTime));
		}

	}

}