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
	public abstract class World : Node, ComponentOf<WorldLine>
		, AsComponentBranch
		, AsChildBranch
		, AsAwake
	{
	}

	public static partial class WorldRule
	{
		class Awake : AwakeRule<World>
		{
			protected override void Execute(World self)
			{
				self.World = self;
			}
		}


	}
}
