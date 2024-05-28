using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OperateLinkFile.Windows
{
	/// <summary>
	/// NewFileControl.xaml 的交互逻辑
	/// </summary>
	public partial class InputFieldControl : UserControl
	{
		/// <summary>
		/// 确认输入框内容
		/// </summary>
		public bool IsOk = false;

		public InputFieldControl()
		{
			InitializeComponent();
			// 添加键盘按键事件处理器
			this.KeyDown += Control_KeyDown;

			// 设置焦点
			TextBox.Focus();
		}
		private void Control_KeyDown(object sender, KeyEventArgs e)
		{
			// 检查是否按下了回车键
			if (e.Key == Key.Enter)
			{
				//检测输入框是否为空
				if (string.IsNullOrEmpty(TextBox.Text)) return;
				IsOk = true;
				Window.GetWindow(this).Close();
			}
			//检测是否按下了ESC键
			if (e.Key == Key.Escape)
			{
				Window.GetWindow(this).Close();
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			//检测输入框是否为空
			if (string.IsNullOrEmpty(TextBox.Text)) return;
			IsOk = true;
			Window.GetWindow(this).Close();
		}

	}
}
