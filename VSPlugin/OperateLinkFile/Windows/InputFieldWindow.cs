using Microsoft.VisualStudio.PlatformUI;
using OperateLinkFile.Windows;
using System;

namespace OperateLinkFile
{
	public class InputFieldWindow : DialogWindow
	{

		/// <summary>
		/// 回调事件
		/// </summary>
		public Action<InputFieldControl, bool> callback;

		public InputFieldWindow()
		{
			InputFieldControl control = new InputFieldControl();
			this.Content = control;

			this.Width = 500;
			this.Height = 80;
			ResizeMode = System.Windows.ResizeMode.NoResize;
			Background = System.Windows.Media.Brushes.Black;
			this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
			this.WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;

			Closed += OnWindowClosing;

		}

		private void OnWindowClosing(object obj, EventArgs e)
		{
			if (Content is InputFieldControl InputFieldControl)
			{
				callback?.Invoke(InputFieldControl, (Content as InputFieldControl).IsOk);
			}
		}
	}
}
