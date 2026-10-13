/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/31 18:06

* 描述： 引用解除法则
* 
* 在我引用的节点回收时触发事件

*/

namespace WorldTree
{
	/// <summary>
	/// 引用子关系解除法则
	/// </summary>
	public interface DeReferencedChild : ISendRule<INode>, ILifeCycleRule { }

	/// <summary>
	/// 引用父关系解除法则
	/// </summary>
	public interface DeReferencedParent : ISendRule<INode>, ILifeCycleRule { }

	/// <summary>
	/// 引用子关系移除法则
	/// </summary>
	public interface ReferencedChildRemove : ISendRule<INode>, ILifeCycleRule { }
	
	/// <summary>
	/// 引用父关系移除法则
	/// </summary>
	public interface ReferencedParentRemove : ISendRule<INode>, ILifeCycleRule { }
}
