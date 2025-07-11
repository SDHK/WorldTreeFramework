using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Reactive;

namespace WorldTree
{
	/// <summary>
	/// ���ڱ�����
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
		/// �������ı�
		/// </summary>
		public string Title
		{
			get => SystemTitle.Text;
			set => SystemTitle.Text = value;
		}

		/// <summary>
		/// ������ɫ
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
		/// ���Ĵ���״̬�仯
		/// </summary>
		private void OnAttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
		{
			SubscribeToWindowState();
		}

		/// <summary>
		/// �������϶�����
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
		/// �رմ���
		/// </summary>
		private void CloseWindow(object sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			Window hostWindow = (Window)this.VisualRoot;
			hostWindow.Close();
		}

		/// <summary>
		/// �������
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
		/// ������С��
		/// </summary>
		private void MinimizeWindow(object sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			Window hostWindow = (Window)this.VisualRoot;
			hostWindow.WindowState = WindowState.Minimized;
		}

		/// <summary>
		/// ����״̬����
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
					//maximizeToolTip.Content = "���";
				}
				if (s == WindowState.Maximized)
				{
					maximizeIcon.Data = this.FindResource("RestoreIconGeometry") as Geometry;
					hostWindow.Padding = new Thickness(7, 7, 7, 7);
					//maximizeToolTip.Content = "��ԭ";
				}
			}));
		}
	}
}
