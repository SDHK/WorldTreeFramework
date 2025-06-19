using Avalonia.Controls;
using System;
using WorldTree;

namespace App
{
	public partial class MainWindow : Window
	{
		/// <summary>
		/// 世界更新时间
		/// </summary>
		public static DateTime UpdateTime;
		public MainWindow()
		{
			InitializeComponent();

			//Type ruleType = typeof(MainWorldRule);
			//Type nodeType = typeof(MainWorld);

			WorldLineManager lineManager = new();
			lineManager.Options = new();
			lineManager.LogType = typeof(WorldLog);
			lineManager.Create(0, typeof(AvaloniaWorldHeart), 1000, typeof(MainWorld));
			//lineManager.MainLine.GetGlobalRuleExecutor(out IRuleExecutor<WinFromEntry> globalRuleExecutor);
			//globalRuleExecutor.Send((Form)this); //发送窗口到全局法则


		}
	}
}