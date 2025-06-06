using WorldTree;

namespace InputBind
{
	internal static class Program
	{
		/// <summary>
		/// 窗口
		/// </summary>
		private static InputBindForm form = new InputBindForm();

		/// <summary>
		/// 世界更新时间
		/// </summary>
		public static DateTime UpdateTime;

		[STAThread]
		static void Main()
		{
			Type ruleType = typeof(MainWorldRule);
			Type nodeType = typeof(MainWorld);

			ApplicationConfiguration.Initialize();
			WorldLineManager lineManager = new();
			lineManager.Options = new();
			lineManager.LogType = typeof(WorldLog);
			lineManager.Create(0, typeof(WinFormWorldHeart), 1000, typeof(MainWorld));
			Application.Run(form);

			//添加节点，节点接收事件拿到From

			UpdateTime = DateTime.Now;

			while (form.IsOpen)
			{
				Thread.Sleep(1000);
				lineManager.MainUpdate?.Invoke(DateTime.Now - UpdateTime);
				UpdateTime = DateTime.Now;
			}
		}
	}
}