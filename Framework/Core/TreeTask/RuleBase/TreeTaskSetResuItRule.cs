/****************************************

* 作者：闪电黑客
* 日期：2024/6/17 10:23

* 描述：

*/
namespace WorldTree
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