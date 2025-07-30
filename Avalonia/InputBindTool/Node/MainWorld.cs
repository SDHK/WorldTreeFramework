using System.Collections.ObjectModel;

namespace WorldTree
{
	/// <summary>
	///	Avalonia主世界
	/// </summary>
	public partial class MainWorld : World
		, AsComponentBranch
		, ComponentOf<World>
		, AsAwake<MainWindow>
		, AsUpdate
	{
		/// <summary>
		/// 窗口
		/// </summary>
		public MainWindow Window;

		/// <summary>
		/// a
		/// </summary>
		public string Header { get; set; }
		/// <summary>
		/// 页签
		/// </summary>
		public ObservableCollection<string> PageNameList { get; } = new() { "第一页", "第二页" };

		/// <summary>
		/// 第二层页签
		/// </summary>
		public ObservableCollection<string> PageName2List { get; } = new() { "第一页1", "第二页2" };

	}
}
