using System;

namespace WorldTree
{
	/// <summary>
	/// DrawGizmos法则接口
	/// </summary>
	public interface IDrawGizmosUpdateTimeRule:ISendRuleBase<TimeSpan> { }

	/// <summary>
	/// DrawGizmos法则
	/// </summary>
	public abstract class DrawGizmosUpdateTimeRule<T> : SendRuleBase<T, IDrawGizmosUpdateTimeRule, TimeSpan> where T : class, INode, AsRule<IDrawGizmosUpdateTimeRule> { }

	/// <summary>
	/// DrawGizmos法则接口
	/// </summary>
	public interface IDrawGizmosUpdateRule : ISendRuleBase { }

	/// <summary>
	/// DrawGizmos法则
	/// </summary>
	public abstract class DrawGizmosUpdateRule<T> : SendRuleBase<T, IDrawGizmosUpdateRule> where T : class, INode, AsRule<IDrawGizmosUpdateRule> { }


}
