using System;

namespace WorldTree
{
	/// <summary>
	/// FixedUpdate法则
	/// </summary>
	public interface FixedUpdate : ISendRuleBase, ILifeCycleRule { }

	/// <summary>
	/// FixedUpdate法则
	/// </summary>
	public interface FixedUpdateTime : ISendRuleBase<TimeSpan>, ILifeCycleRule { }
}
