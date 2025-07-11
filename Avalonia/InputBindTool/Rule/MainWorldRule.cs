﻿
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Styling;
using System;

namespace WorldTree
{
	public static partial class MainWorldRule
	{
		[NodeRule(nameof(AwakeRule<MainWorld, MainWindow>))]
		private static void OnAwake(this MainWorld self, MainWindow window)
		{
			window.DataContext = self;

			StyleInclude darkTheme;
			darkTheme = new StyleInclude(new Uri("avares://Node"))
			{
				Source = new Uri("avares://Avalonia.Core/View/Style/DarkTheme.axaml")
			};
			window.Styles.Add(darkTheme);


			self.Log("Avalonia入口！！");

			self.Window = window;
			self.Window.TestText.Text = "Hello World!";

			self.Core.RealTimeManager.TimeZone = 8; // 设置时区为东八区


			self.Window.titleBar.Title = "测试标题";

			var tab = new TabItem
			{
				Header = new TextBlock { Text = "Tab1" },
				Content = new InputBindPage()
			};

			var tab2 = new TabItem
			{
				Header = new TextBlock { Text = "Tab2" },
				Content = new InputBindPage()
			};

			self.Window.MyTabControl.Items.Add(tab);
			self.Window.MyTabControl.Items.Add(tab2);



		}

		[NodeRule(nameof(UpdateRule<MainWorld>))]
		private static void OnUpdate(this MainWorld self)
		{
			self.Window.TestText.Text = $"{self.Core.RealTimeManager.Now}";
			self.Log($"{self.Window.TestText.Text}");
		}
	}
}
