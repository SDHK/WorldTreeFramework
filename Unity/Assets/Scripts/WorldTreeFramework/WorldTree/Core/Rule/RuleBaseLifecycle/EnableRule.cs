/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/24 17:30

* 描述： 活跃启用法则
*
* 会在加入节点后触发事件

*/

namespace WorldTree
{
	/// <summary>
	/// 活跃启用法则
	/// </summary>
	public interface Enable : ISendRule, ILifeCycleRule, ISourceGeneratorIgnore
	{ }

	/// <summary>
	/// 活跃启用法则
	/// </summary>
	public abstract class EnableRule<N> : SendRule<N, Enable>
	where N : class, INode, AsRule<Enable>
	{
		public override void Invoke(INode self)
		{
			if (self.IsActive != self.m_ActiveEventMark)
			{
				if (self.IsActive)
				{
					if (this.RuleIndex == this.RuleCount - 1) { self.m_ActiveEventMark = self.IsActive; }
					Execute(self as N);
				}
			}
		}
	}

	public delegate void OnEnable<N>(N self) where N : class, INode, AsRule<Enable>;
}