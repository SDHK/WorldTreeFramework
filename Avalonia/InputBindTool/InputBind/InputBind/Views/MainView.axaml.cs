using Avalonia.Controls;
using WorldTree;

namespace InputBind.Views
{
	public partial class MainView : UserControl
	{
		public MainView()
		{
			InitializeComponent();

			WorldLineManager lineManager = new();

		}
	}
}