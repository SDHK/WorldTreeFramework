using System;

namespace WorldTree
{

	/// <summary>
	/// Program
	/// </summary>
	internal class Program
	{
		/// <summary>
		/// 启动
		/// </summary>
		private static void Main(string[] args)
		{
			WorldTreeCore core = new WorldTreeCore();

			core.Log = Console.WriteLine;
			core.LogWarning = Console.WriteLine;
			core.LogError = Console.Error.WriteLine;
			core.Awake();

			core.Root.AddComponent(out WorldHeart _, 1000).Run();//启动世界心跳 设定间隔为1000ms

			core.Root.AddComponent(out Entry _);
			Type ruleType = typeof(EntryRule);
		}
	}
}