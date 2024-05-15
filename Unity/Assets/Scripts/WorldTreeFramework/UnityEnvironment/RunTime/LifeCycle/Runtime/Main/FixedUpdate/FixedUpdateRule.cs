using System;

namespace WorldTree
{
	/// <summary>
	/// FixedUpdate法则
	/// </summary>
	public interface FixedUpdate : ISendRule, ILifeCycleRule { }

	/// <summary>
	/// FixedUpdate法则
	/// </summary>
	public interface FixedUpdateTime : ISendRule<TimeSpan>, ILifeCycleRule { }
}
