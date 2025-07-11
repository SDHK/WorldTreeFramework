using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Reactive;

namespace WorldTree
{
	/// <summary>
	/// 窗口标题栏
	/// </summary>
	public partial class WindowTitleBar : StackPanel
	{
		/// <summary>
		/// a
		/// </summary>
		private Path maximizeIcon;

		/// <summary>
		/// a
		/// </summary>
		public static readonly StyledProperty<IBrush> ForegroundProperty =
	AvaloniaProperty.Register<WindowTitleBar, IBrush>(nameof(Foreground));


		/// <summary>
		/// 标题栏文本
		/// </summary>
		public string Title
		{
			get => SystemTitle.Text;
			set => SystemTitle.Text = value;
		}

		/// <summary>
		/// 字体颜色
		/// </summary>
		public IBrush Foreground
		{
			get => SystemTitle.Foreground;
			set => SystemTitle.Foreground = value;
		}

		public WindowTitleBar()
		{
			InitializeComponent();

			this.maximizeIcon = this.MaximizeButton.FindControl<Path>("MaximizeIcon");
			//this.maximizeToolTip = this.MaximizeButton.FindControl<ToolTip>("MaximizeToolTip");

			this.CloseButton.Click += CloseWindow;
			this.MaximizeButton.Click += MaximizeWindow;
			this.MinimizeButton.Click += MinimizeWindow;
			this.PointerPressed += WindowTitleBar_PointerPressed;
			this.AttachedToVisualTree += OnAttachedToVisualTree;
		}

		/// <summary>
		/// 订阅窗口状态变化
		/// </summary>
		private void OnAttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
		{
			SubscribeToWindowState();
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
		private void SubscribeToWindowState()
		{
			if (VisualRoot is not Window hostWindow) return;
			hostWindow.GetObservable(Window.WindowStateProperty).Subscribe(new AnonymousObserver<WindowState>(s =>
			{
				if (s != WindowState.Maximized)
				{
					maximizeIcon.Data = this.FindResource("MaximizeIconGeometry") as Geometry;
					hostWindow.Padding = new Thickness(0, 0, 0, 0);
					//maximizeToolTip.Content = "最大化";
				}
				if (s == WindowState.Maximized)
				{
					maximizeIcon.Data = this.FindResource("RestoreIconGeometry") as Geometry;
					hostWindow.Padding = new Thickness(7, 7, 7, 7);
					//maximizeToolTip.Content = "还原";
				}
			}));
		}
	}
}
