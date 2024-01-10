using System;

namespace WorldTree
{

	/// <summary>
	/// OnGUI法则接口
	/// </summary>
	public interface IGuiUpdateTimeRule : ISendRuleBase<TimeSpan> { }

	/// <summary>
	/// OnGUI法则
	/// </summary>
	public abstract class GuiUpdateTimeRule<T> : SendRuleBase<T, IGuiUpdateTimeRule, TimeSpan> where T : class, INode, AsRule<IGuiUpdateTimeRule> { }

	/// <summary>
	/// OnGUI法则接口
	/// </summary>
	public interface IGuiUpdateRule : ISendRuleBase { }

	/// <summary>
	/// OnGUI法则
	/// </summary>
	public abstract class GuiUpdateRule<T> : SendRuleBase<T, IGuiUpdateRule> where T : class, INode, AsRule<IGuiUpdateRule> { }
}
