using System;
using System.Windows.Forms;
using WorldTree;

namespace InputBind
{
	internal static class Program
	{
		/// <summary>
		/// ´°¿Ú
		/// </summary>
		private static InputBindForm form = new InputBindForm();


		[STAThread]
		static void Main()
		{
			Type ruleType = typeof(MainWorldRule);
			Type nodeType = typeof(MainWorld);

			ApplicationConfiguration.Initialize();
			Application.Run(form);
		}
	}
}