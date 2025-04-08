/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/14 20:30

* 描述： 世界

*/

namespace WorldTree
{

	/// <summary>
	/// 世界
	/// </summary>
	public class World : Node, ComponentOf<WorldLine>
		, AsComponentBranch
		, AsChildBranch
		, AsAwake
	{
	}
}
