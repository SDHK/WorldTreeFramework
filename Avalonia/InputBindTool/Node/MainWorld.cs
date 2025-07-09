namespace WorldTree
{
	/// <summary>
	///	Avalonia主世界
	/// </summary>
	public partial class MainWorld : World
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
namespace WorldTree
{
	/// <remarks>
	/// <para>parent：<see cref="WorldTree.WorldLine"/> </para>
	/// <para>-<see cref="WorldTree.TimerCall"/> </para>
	/// <para>-<see cref="WorldTree.TimerCycle"/> </para>
	/// </remarks>
	public partial class MainWorld;

}
