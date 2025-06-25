using Avalonia.Controls;

namespace WorldTree
{
	/// <summary>
	/// ������
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			WorldLineManager lineManager = new();
			lineManager.Options = new();

			//�ж��Ƿ�Ϊ���ģʽ������ǣ���ʹ����־��
			if (!Design.IsDesignMode) lineManager.LogType = typeof(WorldLog);

			var line = lineManager.Create(0, typeof(AvaloniaWorldHeart), 1000);
			line.WorldContext.Post(() =>
			{
				line.AddComponent(out MainWorld _, this);
			});
		}
	}
}