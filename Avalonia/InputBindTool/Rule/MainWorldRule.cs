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
			self.World.AddComponent(out InputManager inputManager);
			self.AddComponent(out InputBindPageViewModel vm, window.InputBindPage);

			inputManager.AddGeneric("第一页", out InputArchive inputArchive);
			inputManager.AddGeneric("第二页", out InputArchive _);


			NodeRuleHelper.SendRule(self, default(UpdateTest));
		}

		[NodeRule(nameof(UpdateRule<MainWorld>))]
		private static void OnUpdate(this MainWorld self)
		{
			self.Window.TestText.Text = $"{self.Core.RealTimeManager.Now}";
			self.Log($"{self.Window.TestText.Text}");
		}


		public class UpdateTest : UpdateRule<MainWorld>
		{
			public override void OnCreate()
			{
				NodeType = Core.TypeToCode(typeof(MainWorld));
				RuleType = Core.TypeToCode(typeof(UpdateTest));
			}
			protected override void Execute(MainWorld self)
			{
				self.Update();
				self.Log($"精确执行Rule");

			}
		}
	}
}
