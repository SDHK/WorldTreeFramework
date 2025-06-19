using ReactiveUI;
using System.Diagnostics;

namespace AvaloniaMVVMTest.ViewModels
{
	public partial class MainWindowViewModel : ReactiveObject
	{

		public string Greeting { get; } = "Welcome to Avalonia!";


		public string consoleText = "";
		public string ConsoleText
		{
			get => consoleText;
			set => this.RaiseAndSetIfChanged(ref consoleText, value);
		}


		public MainWindowViewModel()
		{
		}


		public void ClickHandler1()
		{

			ConsoleText = "ClickHandler1按钮被点击了！";
			Debug.WriteLine("Click!");
		}

		public void ButtonAction()
		{
			ConsoleText = "ButtonAction按钮被点击了！";
			Debug.WriteLine("ButtonAction!");
		}


	}
}
