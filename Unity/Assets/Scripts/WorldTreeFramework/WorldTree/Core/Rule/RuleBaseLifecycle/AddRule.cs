/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/1 9:47

* 描述： 添加法则
*
* 在加入节点时触发事件
*
*
* 曾试图用 IAwakeRule  改名为 IAddRule ，合并这两个看起来差不多的事件。
*
* 后发现不可行，IAddRule 是代表节点加入树的统一事件。
* 并且多态化了，子类可以用父类的AddRule执行，可以用来初始化公共字段。
*
* 而 IAwakeRule 是用于参数传入的，是种可有可无的事件，
* 假如子类节点参数不同，是不会执行父类的IAwakeRule。
* 导致子类必须重新将公共字段的初始化再写一遍。
*

*/

namespace WorldTree
{
	/// <summary>
	/// 添加法则接口
	/// </summary>
	public interface IAddRule : ISendRuleBase
	{ }

	/// <summary>
	/// 添加法则
	/// </summary>
	public abstract class AddRule<N> : SendRuleBase<N, IAddRule> where N : class, INode, AsRule<IAddRule>
	{ }
}