using System;
using System.Windows.Forms;
using WorldTree;
namespace InputBind
{
	public partial class InputBindForm : Form
	{

		/// <summary>
		/// 世界更新时间
		/// </summary>
		public static DateTime UpdateTime;

		public InputBindForm()
		{

			InitializeComponent();

			WorldTreeCore lineManager = new();
			lineManager.SetLog<WorldLog>();
			var line = lineManager.Create(0, typeof(MainWorldWinForm), typeof(WinFormWorldHeart), 1000);
			//line.WorldContext.Post(() =>
			//{
			//	line.TryGetComponent(out MainWorldWinForm form); (Form)this
			//});
		}

		private void Form1_Load(object sender, EventArgs e)
		{


		}

		private void button1_Click(object sender, EventArgs e)
		{
			//Controls
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{

		}
	}





}
