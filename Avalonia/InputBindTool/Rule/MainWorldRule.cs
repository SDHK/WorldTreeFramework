using Avalonia.Controls;
using Node;

namespace WorldTree
{
	public static partial class MainWorldRule
	{
		[NodeRule(nameof(AwakeRule<MainWorld, Window>))]
		private static void OnAwake(this MainWorld self, Window window)
		{
			self.Log("Avalonia入口！！");
			self.Window = window as MainWindow;
			self.Window.TestText.Text = "Hello World!";
		}

		[NodeRule(nameof(UpdateRule<MainWorld>))]
		private static void OnUpdate(this MainWorld self)
		{
			self.Window.TestText.Text = $"{self.Core.RealTimeManager.UtcNow}";
			self.Log($"{self.Window.TestText.Text}");
		}
	}
}
