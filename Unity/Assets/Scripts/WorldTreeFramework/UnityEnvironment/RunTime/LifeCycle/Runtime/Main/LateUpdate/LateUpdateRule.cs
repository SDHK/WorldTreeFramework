using System;

namespace WorldTree
{
	/// <summary>
	/// LateUpdate法则
	/// </summary>
	public interface LateUpdate : ISendRule, ILifeCycleRule { }

	/// <summary>
	/// LateUpdate法则
	/// </summary>
	public interface LateUpdateTime : ISendRule<TimeSpan>, ILifeCycleRule { }

}
