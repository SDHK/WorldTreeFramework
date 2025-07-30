/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/3 23:26

* 描述： 刷新法则

*/

using System;

namespace WorldTree
{
	/// <summary>
	/// 刷新法则
	/// </summary>
	/// <remarks>
	/// <para>
	///  <see cref="TimeSpan"/> 是间隔时间
	/// </para>
	/// </remarks>
	public interface UpdateTime : ISendRule<TimeSpan>, ILifeCycleRule
	{ }

	/// <summary>
	/// 刷新法则
	/// </summary>
	public interface Update : ISendRule, ILifeCycleRule
	{ }

	/// <summary>
	/// UpdateRule补充类
	/// </summary>
	public static class UpdateRuleSupplement
	{
		/// <summary>
		/// 刷新法则
		/// </summary>
		/// <remarks>
		/// <Para>
		/// 执行通知法则: <see cref="WorldTree.Update"/> : <see cref="ISendRule"/>
		/// </Para>
		/// </remarks>
		public static void Update<N>(this N self) where N : class, INode, AsRule<Update> => NodeRuleHelper.SendRule(self, default(Update));
	}
}