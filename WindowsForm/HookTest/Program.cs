using WorldTree;

namespace HookTest
{
	internal class Program
	{
		private static Form1 form1 = new Form1();

		public static int a = 0;

		public static WorldLine mainLine = new WorldLine();

		private static void Main(string[] args)
		{

			form1.labelT.Text = $"启动:{a++}";

			mainLine.Init(new WorldLineSetting());

			mainLine.World.AddComponent(out Entry _);

			Type ruleType = typeof(EntryRule);//防止程序集被优化掉
			Type nodeType = typeof(DotNetInit);

			form1.TextBox.Text = NodeRule.ToStringDrawTree(mainLine);
			Application.Run(form1);
		}
	}
}