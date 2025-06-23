using Avalonia.Controls;

namespace WorldTree
{
	/// <summary>
	/// Avalonia主世界
	/// </summary>
	public class MainWorld : World
		, AsComponentBranch
		, ComponentOf<World>
		, AsAwake<Window>
	{

	}
}
