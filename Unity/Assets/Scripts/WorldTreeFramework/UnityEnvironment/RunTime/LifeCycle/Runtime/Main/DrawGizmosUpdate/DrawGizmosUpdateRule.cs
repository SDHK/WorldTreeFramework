using System;

namespace WorldTree
{
	/// <summary>
	/// DrawGizmos法则
	/// </summary>
	public interface DrawGizmosUpdateTime : ISendRule<TimeSpan>, ILifeCycleRule { }

	/// <summary>
	/// DrawGizmos法则
	/// </summary>
	public interface DrawGizmosUpdate : ISendRule, ILifeCycleRule { }
}
