using Avalonia.Controls;

namespace WorldTree
{
	/// <summary>
	/// 主窗口
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			WorldLineManager lineManager = new();
			lineManager.Options = new();

			//判断是否为设计模式，如果是，则不使用日志。
			if (!Design.IsDesignMode) lineManager.LogType = typeof(WorldLog);

			var line = lineManager.Create(0, typeof(AvaloniaWorldHeart), 1000);
			line.WorldContext.Post(() =>
			{
				line.AddComponent(out MainWorld _, this);
			});
		}
	}
}