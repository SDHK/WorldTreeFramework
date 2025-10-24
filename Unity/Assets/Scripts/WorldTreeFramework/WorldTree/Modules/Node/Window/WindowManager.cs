/****************************************

* 作者： 闪电黑客
* 日期： 2025/7/17 20:46

* 描述： 

*/
namespace WorldTree
{
	/// <summary>
	/// 窗口管理器
	/// </summary>
	public class WindowManager : Node
		, ComponentOf<World>
		, AsComponentBranch
		, AsChildBranch
		, AsRule<Awake>
	{
		/// <summary>
		/// 全部窗口
		/// </summary>
		public UnitDictionary<long, INode> windowDict = new UnitDictionary<long, INode>();
	}
}
