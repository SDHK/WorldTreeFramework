using Avalonia.Controls;
using System;
using WorldTree;

namespace App
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			Type ruleType = typeof(MainWorldRule);
			Type nodeType = typeof(MainWorld);

			WorldLineManager lineManager = new();
			lineManager.Options = new();
			lineManager.LogType = typeof(WorldLog);
			var line = lineManager.Create(0, typeof(AvaloniaWorldHeart), 1000);
			line.WorldContext.Post(() =>
			{
				line.AddComponent(out MainWorld _, (Window)this);
			});
		}
	}
}