using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Reactive;
using System.Threading.Tasks;

namespace WorldTree
{
	/// <summary>
	/// 窗口标题栏
	/// </summary>
	public partial class WindowTitleBar : UserControl
	{
		/// <summary>
		/// a
		/// </summary>
		private Path maximizeIcon;

		/// <summary>
		/// a
		/// </summary>
		private ToolTip maximizeToolTip;

		//private Button minimizeButton;
		//private Button maximizeButton;
		//private Button closeButton;
		//private Image windowIcon;


		public WindowTitleBar()
		{
			InitializeComponent();

			this.CloseButton.Click += CloseWindow;
			this.MaximizeButton.Click += MaximizeWindow;
			this.MinimizeButton.Click += MinimizeWindow;
			this.maximizeIcon = this.MaximizeButton.FindControl<Path>("MaximizeIcon");
			this.maximizeToolTip = this.MaximizeButton.FindControl<ToolTip>("MaximizeToolTip");

			this.PointerPressed += WindowTitleBar_PointerPressed;

			SubscribeToWindowState();
		}

		/// <summary>
		/// 关闭窗口
		/// </summary>
		private void CloseWindow(object sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			Window hostWindow = (Window)this.VisualRoot;
			hostWindow.Close();
		}

		/// <summary>
		/// 窗口最大化
		/// </summary>
		private void MaximizeWindow(object sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			Window hostWindow = (Window)this.VisualRoot;

			if (hostWindow.WindowState == WindowState.Normal)
			{
				hostWindow.WindowState = WindowState.Maximized;
			}
			else
			{
				hostWindow.WindowState = WindowState.Normal;
			}
		}
		/// <summary>
		/// 窗口最小化
		/// </summary>
		private void MinimizeWindow(object sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			Window hostWindow = (Window)this.VisualRoot;
			hostWindow.WindowState = WindowState.Minimized;
		}

		/// <summary>
		/// 窗口状态订阅
		/// </summary>
		private async void SubscribeToWindowState()
		{
			Window hostWindow = (Window)this.VisualRoot;

			while (hostWindow == null)
			{
				hostWindow = (Window)this.VisualRoot;
				await Task.Delay(50);
			}

			hostWindow.GetObservable(Window.WindowStateProperty).Subscribe(new AnonymousObserver<WindowState>(s =>
			{
				if (s != WindowState.Maximized)
				{
					maximizeIcon.Data = Geometry.Parse("M2048 2048v-2048h-2048v2048h2048zM1843 1843h-1638v-1638h1638v1638z");
					hostWindow.Padding = new Thickness(0, 0, 0, 0);
					maximizeToolTip.Content = "Maximize";
				}
				if (s == WindowState.Maximized)
				{
					maximizeIcon.Data = Geometry.Parse("M2048 1638h-410v410h-1638v-1638h410v-410h1638v1638zm-614-1024h-1229v1229h1229v-1229zm409-409h-1229v205h1024v1024h205v-1229z");
					hostWindow.Padding = new Thickness(7, 7, 7, 7);
					maximizeToolTip.Content = "Restore Down";
				}
			}));
		}

		/// <summary>
		/// 标题栏拖动窗口
		/// </summary>
		private void WindowTitleBar_PointerPressed(object sender, PointerPressedEventArgs e)
		{
			Window hostWindow = this.VisualRoot as Window;
			if (hostWindow != null)
			{
				hostWindow.BeginMoveDrag(e);
			}
		}
	}
}
