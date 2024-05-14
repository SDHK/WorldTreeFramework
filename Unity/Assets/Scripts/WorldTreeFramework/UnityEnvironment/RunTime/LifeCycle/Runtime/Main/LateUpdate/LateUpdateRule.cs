using System;

namespace WorldTree
{
	/// <summary>
	/// LateUpdate法则
	/// </summary>
	public interface LateUpdate : ISendRuleBase, ILifeCycleRule { }

	/// <summary>
	/// LateUpdate法则
	/// </summary>
	public interface LateUpdateTime : ISendRuleBase<TimeSpan>, ILifeCycleRule { }

}
