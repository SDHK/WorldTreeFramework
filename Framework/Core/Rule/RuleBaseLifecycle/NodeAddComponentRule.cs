/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/18 9:35

* 描述： 给节点添加组件的法则
*
* 用于节点的饿汉单例
*
* 法则是可多播的，执行顺序由名字排序
*

*/

namespace WorldTree
{
	/// <summary>
	/// 给节点添加组件的法则
	/// </summary>
	/// <typeparam name="N">节点</typeparam>
	/// <typeparam name="C">组件</typeparam>
	/// <remarks>用于节点的饿汉单例</remarks>
	public abstract class NodeAddComponentRule<N, C> : AddRule<N>
		where N : class, INode, AsRule<Add>, AsComponentBranch
		where C : class, INode, ComponentOf<N>, AsRule<Awake>
	{
		protected override void Execute(N self)
		{
			self.AddComponent(out C c_);
		}
	}
}