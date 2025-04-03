using WorldTree;

namespace HookTest
{
	internal class Program
	{
		private static Form1 form1 = new Form1();

		public static int a = 0;

		public static WorldTreeCore core = new WorldTreeCore();

		private static void Main(string[] args)
		{

			form1.labelT.Text = $"启动:{a++}";

			core.Log = Console.WriteLine;
			core.LogWarning = Console.WriteLine;
			core.LogError = Console.Error.WriteLine;
			//启动世界心跳 设定间隔为1000ms
			core.Init(typeof(WorldHeart), 1000);

			core.World.AddComponent(out Entry _);

			Type ruleType = typeof(EntryRule);//防止程序集被优化掉
			Type nodeType = typeof(DotNetInit);

			form1.TextBox.Text = NodeRule.ToStringDrawTree(core);
			Application.Run(form1);
		}
	}
}