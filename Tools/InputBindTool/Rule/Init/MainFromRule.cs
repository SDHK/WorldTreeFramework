using System.Drawing;
using System.Windows.Forms;

namespace WorldTree
{
	public static partial class MainFromRule
	{
		[NodeRule(nameof(WinFromEntryRule<MainFrom>))]
		private static void OnWinFromEntry(this MainFrom self, Form arg1)
		{

			// 创建 TabControl 实例
			TabControl1 tabControl1 = new();

			// 设置基本属性
			tabControl1.Dock = DockStyle.Fill;  // 填充整个窗体
			tabControl1.Location = new Point(10, 10);
			tabControl1.Size = new Size(400, 300);
			tabControl1.Appearance = TabAppearance.Normal;  // 或 TabAppearance.Buttons
			tabControl1.SizeMode = TabSizeMode.Normal;  // 或 TabSizeMode.Fixed, TabSizeMode.FillToRight
			tabControl1.Alignment = TabAlignment.Top;


			tabControl1.BackColor = Color.FromArgb(30, 30, 30);
			tabControl1.ForeColor = Color.FromArgb(30, 30, 30);
			//设置选项按钮的背景色


			CustomTabPage tabPage1 = new CustomTabPage();
			tabPage1.Text = "选项卡1";  // 设置选项卡页的标题

			//tabPage1.BackColor = Color.LightBlue;

			// 设置选项卡页的背景颜色
			tabPage1.BackColor = Color.FromArgb(30, 30, 30);
			tabPage1.ForeColor = Color.FromArgb(30, 30, 30);

			// 添加控件到第一个选项卡页
			Label lblName = new Label();
			lblName.Text = "姓名:";
			lblName.Location = new Point(20, 20);
			tabPage1.Controls.Add(lblName);

			TextBox txtName = new TextBox();
			txtName.Location = new Point(100, 20);
			txtName.Width = 150;
			tabPage1.Controls.Add(txtName);

			// 创建第二个选项卡页
			CustomTabPage tabPage2 = new CustomTabPage();
			tabPage2.Text = "选项卡2";  // 设置选项卡页的标题
									 //tabPage2.BackColor = Color.LightGreen;
			tabPage2.BackColor = Color.FromArgb(30, 30, 30);

			// 添加控件到第二个选项卡页
			Label lblEmail = new Label();
			lblEmail.Text = "邮箱:";
			lblEmail.Location = new Point(20, 20);
			tabPage2.Controls.Add(lblEmail);

			TextBox txtEmail = new TextBox();
			txtEmail.Location = new Point(100, 20);
			txtEmail.Width = 150;
			tabPage2.Controls.Add(txtEmail);

			// 将选项卡页添加到 TabControl
			tabControl1.TabPages.Add(tabPage1);
			tabControl1.TabPages.Add(tabPage2);

			//arg1.Controls.Add(tabControl1);
			var panel = new Panel();
			panel.Name = "panel1";
			//panel.Dock = DockStyle.Fill;
			panel.Location = new Point(10, 10);
			panel.Size = new Size(400, 300);
			panel.TabIndex = 1;


			var treeView = new NavigationBar("选项1", "选项2");
			panel.Controls.Add(treeView);

			var listBox = new ListBox
			{
				Name = "listBox1",
				Location = new Point(100, 10),
				Size = new Size(120, 95),
				Items =
				{
					"Item 1",
					"Item 2",
					"Item 3"
				}
			};


			//arg1.Controls.Add(panel);
			arg1.Controls.Add(listBox);


			//SetDarkTheme(arg1);
		}

		/// <summary>
		/// 设置控件及其子控件的深色主题样式。
		/// </summary>
		public static void SetDarkTheme(Control control)
		{
			control.BackColor = Color.FromArgb(30, 30, 30);
			control.ForeColor = Color.White;
			foreach (Control child in control.Controls)
				SetDarkTheme(child);
		}
	}
}
