using System.Windows;

namespace InputMapperTool
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			this.StateChanged += MainWindow_StateChanged;
		}
		private void MainWindow_StateChanged(object sender, EventArgs e)
		{
			// 更新最大化/还原按钮的图标
			if (WindowState == WindowState.Maximized)
			{
				MaximizeIcon.Visibility = Visibility.Collapsed;
				RestoreIcon.Visibility = Visibility.Visible;
			}
			else
			{
				MaximizeIcon.Visibility = Visibility.Visible;
				RestoreIcon.Visibility = Visibility.Collapsed;
			}
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{

			Close();
		}

		private void MinimizeButton_Click(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		private void MaximizeButton_Click(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState == WindowState.Maximized ?
			WindowState.Normal : WindowState.Maximized;
		}


	}
}