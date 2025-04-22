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

			form1.labelT.Text = $"Æô¶¯:{a++}";


			//form1.TextBox.Text = NodeRule.ToStringDrawTree(mainLine);
			Application.Run(form1);
		}
	}
}