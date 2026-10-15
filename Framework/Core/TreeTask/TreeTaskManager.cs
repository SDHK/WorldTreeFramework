/****************************************

* 作者：闪电黑客
* 日期：2026/10/15

* 描述：

*/
namespace WorldTree
{
	/// <summary>
	/// 树任务挂载点。挂在世界上，本身没有逻辑，任务都挂在这里。
	/// </summary>
	public class TreeTaskManager : Node, ComponentOf<World>
		, AsRule<Awake>
	{
		/// <summary>
		/// 取世界上的挂载点，没有就创建一个。
		/// </summary>
		public static TreeTaskManager Get(INode node)
		{
			if (!node.World.TryGetComponent(out TreeTaskManager manager))
				node.World.AddComponent(out manager);
			return manager;
		}
	}
}
