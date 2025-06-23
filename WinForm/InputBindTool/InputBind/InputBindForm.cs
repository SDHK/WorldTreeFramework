using System;
using System.Windows.Forms;
using WorldTree;
namespace InputBind
{
	public partial class InputBindForm : Form
	{

		/// <summary>
		/// �������ʱ��
		/// </summary>
		public static DateTime UpdateTime;

		public InputBindForm()
		{

			InitializeComponent();

			WorldLineManager lineManager = new();
			lineManager.Options = new();
			lineManager.LogType = typeof(WorldLog);
			var line = lineManager.Create(0, typeof(WinFormWorldHeart), 1000);
			line.WorldContext.Post(() =>
			{
				line.AddComponent(out MainWorldWinForm _, (Form)this);
			});
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
