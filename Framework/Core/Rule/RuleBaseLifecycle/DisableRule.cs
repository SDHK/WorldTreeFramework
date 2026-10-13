/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/24 17:31

* 描述： 失活法则
*
* 同时会在移除节点前触发事件
*

*/

namespace WorldTree
{
	/// <summary>
	/// 失活法则
	/// </summary>
	public interface Disable : ISendRule, ILifeCycleRule, ISourceGeneratorIgnore
	{ }

	/// <summary>
	/// 失活法则
	/// </summary>
	public abstract class DisableRule<N> : SendRule<N, Disable>
		where N : class, INode, AsRule<Disable>
	{
		public override void Invoke(INode self)
		{
			if (self.IsActive != self.activeEventMark)
			{
				if (!self.IsActive)
				{
					if (this.RuleIndex == this.RuleCount - 1) { self.activeEventMark = self.IsActive; }
					Execute(self as N);
				}
			}
		}
	}
}