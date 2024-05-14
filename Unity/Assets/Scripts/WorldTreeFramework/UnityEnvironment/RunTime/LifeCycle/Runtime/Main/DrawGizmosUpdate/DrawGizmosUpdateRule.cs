using System;

namespace WorldTree
{
	/// <summary>
	/// DrawGizmos法则
	/// </summary>
	public interface DrawGizmosUpdateTime : ISendRuleBase<TimeSpan>, ILifeCycleRule { }

	/// <summary>
	/// DrawGizmos法则
	/// </summary>
	public interface DrawGizmosUpdate : ISendRuleBase, ILifeCycleRule { }
}
