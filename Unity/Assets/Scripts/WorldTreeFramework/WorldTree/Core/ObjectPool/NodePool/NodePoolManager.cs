/****************************************

* 作者： 闪电黑客
* 日期： 2022/7/17 17:23

* 描述： 节点对象池管理器。
* 
*/
namespace WorldTree
{
	/// <summary>
	/// 节点对象池管理器
	/// </summary>
	public class NodePoolManager : PoolManagerBase<NodePool>
		, CoreManagerOf<WorldLine>
		, AsRule<Awake>
	{
	}
}
