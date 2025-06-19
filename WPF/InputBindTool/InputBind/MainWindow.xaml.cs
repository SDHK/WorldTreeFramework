using System.Windows;

namespace InputBind
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			this.ListBox001.Items.Add("New Item!!!!");
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{

		}

		private void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (ListBox001.SelectedItem != null)
			{
				MessageBox.Show($"Selected Item: {ListBox001.SelectedItem}");
			}
			else
			{
				MessageBox.Show("No item selected.");
			}
		}
	}
}