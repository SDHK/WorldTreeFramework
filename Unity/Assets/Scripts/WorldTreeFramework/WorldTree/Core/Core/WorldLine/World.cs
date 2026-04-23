/****************************************

* 作者： 闪电黑客
* 日期： 2023/3/14 20:30

* 描述： 世界

*/

namespace WorldTree
{
	/// <summary>
	/// 世界接口  
	/// </summary>
	public interface IWorld : INode
	{
		/// <summary>
		/// 世界线
		/// </summary>
		public WorldLine WorldLine { get; set; }
	}

	/// <summary>
	/// 世界
	/// </summary>
	public abstract class World : Node, IWorld
		, ComponentOf<WorldLine>
		, AsComponentBranch
		, AsChildBranch
		, AsRule<Awake>
	{

		/// <summary>
		/// 世界线
		/// </summary>
		public WorldLine WorldLine { get; set; }
	}

	public static partial class WorldRule
	{
		class Awake : AwakeRule<World>
		{
			protected override void Execute(World self)
			{
				if (self.Parent is WorldLine)
				{
					self.WorldLine = (WorldLine)self.Parent;
				}
				else
				{
					self.WorldLine = self.Parent.World.WorldLine;
				}
				self.World = self;
			}
		}

	}
}
