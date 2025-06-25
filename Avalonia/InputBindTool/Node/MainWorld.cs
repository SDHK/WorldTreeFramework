using Avalonia.Controls;
using Node;

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
		/// <summary>
		/// 窗口
		/// </summary>
		public MainWindow Window;

	}
}
