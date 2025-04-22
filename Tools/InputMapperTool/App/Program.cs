using NLog;
using WorldTree;

namespace InputMapperTool
{
	internal static class Program
	{
		private static InputMapperForm form = new InputMapperForm();
		private static WorldLine mainLine = new WorldLine();

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

			Type loggerType = typeof(Logger);

		}
	}
}