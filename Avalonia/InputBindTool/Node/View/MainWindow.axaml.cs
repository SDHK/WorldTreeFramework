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

			WorldTreeCore worldTree = new();

			//判断是否为设计模式，如果是，则不使用日志。
			if (!Design.IsDesignMode) worldTree.SetLog<WorldLog>();

			worldTree.Create(0, typeof(MainWorld), typeof(AvaloniaWorldHeart), 1000);
		}
	}
}