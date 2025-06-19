using Avalonia.Controls;
using System.Diagnostics;

namespace AvaloniaMVVMTest.Views
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			DataContext = new ViewModels.MainWindowViewModel();
		}

		private void Button_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			// 这里写点击后的逻辑
			Console.Text = "Button_Click按钮被点击了！";
			Debug.WriteLine("Button_Click按钮被点击了!");
		}


	}
}