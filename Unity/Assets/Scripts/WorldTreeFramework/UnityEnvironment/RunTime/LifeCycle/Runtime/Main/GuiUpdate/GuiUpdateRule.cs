using System;

namespace WorldTree
{
	/// <summary>
	/// OnGUI法则
	/// </summary>
	public interface GuiUpdateTime : ISendRuleBase<TimeSpan>, ILifeCycleRule { }

	/// <summary>
	/// OnGUI法则
	/// </summary>
	public interface GuiUpdate : ISendRuleBase, ILifeCycleRule { }
}
