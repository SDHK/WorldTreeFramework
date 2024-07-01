/****************************************

* 作者： 闪电黑客
* 日期： 2022/8/24 17:30

* 描述： 激活法则
*
* 会在加入节点后触发事件

*/

namespace WorldTree
{
	/// <summary>
	/// 激活法则
	/// </summary>
	public interface Enable : ISendRule, ILifeCycleRule, ISourceGeneratorIgnore
	{ }

	/// <summary>
	/// 激活法则
	/// </summary>
	public abstract class EnableRule<N> : SendRule<N, Enable>
	where N : class, INode, AsRule<Enable>
	{
		public override void Invoke(INode self)
		{
			if (self.IsActive != self.activeEventMark)
			{
				if (self.IsActive)
				{
					if (this.RuleIndex == this.RuleCount - 1) { self.activeEventMark = self.IsActive; }
					Execute(self as N);
				}
			}
		}
	}
	/// <summary>
	/// 激活法则
	/// </summary>
	/// <remarks>
	/// <Para>
	/// 通知法则委托: <see cref="WorldTree.Enable"/> : <see cref="ISendRule"/>
	/// </Para>
	/// </remarks>
	public delegate void OnEnable<N>(N self) where N : class, INode, AsRule<Enable>;
}