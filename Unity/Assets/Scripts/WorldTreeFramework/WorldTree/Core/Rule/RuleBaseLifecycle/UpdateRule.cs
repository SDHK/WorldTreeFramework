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
	//public abstract class UpdateTimeRule<N> : SendRule<N, UpdateTime, TimeSpan> where N : class, INode, AsRule<UpdateTime>
	//{
	//	protected override void Execute(N self, TimeSpan arg1)
	//	{

	//	}
	//}

	/// <summary>
	/// 刷新法则
	/// </summary>
	public interface Update : ISendRule, ILifeCycleRule
	{ }

	///// <summary>
	///// 刷新法则
	///// </summary>
	//public abstract class UpdateRule<N> : SendRule<N, Update> where N : class, INode, AsRule<Update>
	//{

	//	protected override void Execute(N self)
	//	{
	//	}
	//}
}