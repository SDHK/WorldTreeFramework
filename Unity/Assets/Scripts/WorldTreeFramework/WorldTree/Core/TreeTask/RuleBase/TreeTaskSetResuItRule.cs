﻿namespace WorldTree
{
	/// <summary>
	/// 树任务设置结果法则
	/// </summary>
	public interface TreeTaskSetResuIt : ISendRule
	{ }

	/// <summary>
	/// 泛型树任务设置结果法则
	/// </summary>
	public interface TreeTaskSetResuIt<T1> : ISendRule<T1>
	{ }

	/// <summary>
	/// 树任务令牌事件法则
	/// </summary>
	public interface TreeTaskTokenEvent : ISendRule<TokenState>
	{ }
}