
namespace WorldTree
{
	/// <summary>
	/// Avalonia主世界
	/// </summary>
	public class MainWorld : World
		, AsComponentBranch
		, ComponentOf<World>
		, AsAwake<MainWindow>
	{
		/// <summary>
		/// 窗口
		/// </summary>
		public MainWindow Window;
	}
}
