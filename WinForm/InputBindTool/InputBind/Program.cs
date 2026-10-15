using System;
using System.Windows.Forms;

namespace InputBind
{
	internal static class Program
	{
		/// <summary>
		/// 窗口
		/// </summary>
		private static InputBindForm form = new InputBindForm();


		[STAThread]
		static void Main()
		{
			ApplicationConfiguration.Initialize();
			Application.Run(form);
		}
	}
}