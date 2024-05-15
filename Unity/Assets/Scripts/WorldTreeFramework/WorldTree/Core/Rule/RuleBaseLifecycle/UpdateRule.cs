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
	public interface UpdateTime : ISendRule<TimeSpan>, ILifeCycleRule
	{ }

	///// <summary>
	///// 刷新法则
	///// </summary>
	//public abstract class UpdateTimeRule<N> : SendRuleBase<N, UpdateTime, TimeSpan> where N : class, INode, AsRule<UpdateTime>
	//{ }

	/// <summary>
	/// 刷新法则
	/// </summary>
	public interface Update : ISendRule, ILifeCycleRule
	{ }

	///// <summary>
	///// 刷新法则
	///// </summary>
	//public abstract class UpdateRule<N> : SendRuleBase<N, Update> where N : class, INode, AsRule<Update>
	//{ }
}