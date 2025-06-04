using WorldTree;

namespace InputMapperTool
{
	/// <summary>
	/// a
	/// </summary>
	internal static class Program
	{
		/// <summary>
		/// a
		/// </summary>
		private static InputMapperForm form = new InputMapperForm();

		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.

			ApplicationConfiguration.Initialize();
			WorldLineManager lineManager = new();
			lineManager.Options = new();
			lineManager.LogType = typeof(WorldLog);
			lineManager.Create(0, typeof(WorldHeart), 1000, typeof(MainWorld));
			Application.Run(form);
		}
	}

	// ¥Êµµµÿ÷∑ ∂¡»° ±£¥Ê
	// 
}