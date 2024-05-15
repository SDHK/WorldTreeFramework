using System;

namespace WorldTree
{
	/// <summary>
	/// OnGUI法则
	/// </summary>
	public interface GuiUpdateTime : ISendRule<TimeSpan>, ILifeCycleRule { }

	/// <summary>
	/// OnGUI法则
	/// </summary>
	public interface GuiUpdate : ISendRule, ILifeCycleRule { }
}
