namespace InputMapperTool
{
	public partial class InputMapperForm : Form
	{
		public InputMapperForm()
		{
			InitializeComponent();
		}

		private void InputMapperForm_Load(object sender, EventArgs e)
		{
			NewTreeView();
			NewlistBox();
			NewlistView();
			NewTabControl();
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			TreeNode a = e.Node;
		}

		// 假设已有一个TreeView控件名为treeView1
		private void NewTreeView()
		{
			// 清除现有节点
			treeView1.Nodes.Clear();
			// 创建根节点
			TreeNode rootNode = new TreeNode("我的电脑");
			// 将根节点添加到TreeView控件
			treeView1.Nodes.Add(rootNode);

			// 假设有一个方法来获取指定目录下的所有子目录和文件
			// 这里简化为直接添加几个示例子节点
			TreeNode documentsNode = new TreeNode("文档");
			documentsNode.Text = "文档ceshi";
			rootNode.Nodes.Add(documentsNode);
			// ... 添加其他子节点
			// 展开根节点
			rootNode.Expand();
		}

		private void NewlistBox()
		{
			listBox1.Items.Clear();
			listBox1.Items.Add("新项");
			listBox1.Items.Add("新项2");
		}
		private void NewlistView()
		{
			this.listView1.Items.Clear(); //清空列表
			this.listView1.BeginUpdate();   //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度

			for (int i = 0; i < 10; i++)   //添加10行数据
			{
				ListViewItem lvi = new ListViewItem();

				lvi.ImageIndex = i;     //通过与imageList绑定，显示imageList中第i项图标

				lvi.Text = "subitem" + i;

				lvi.SubItems.Add("第2列,第" + i + "行");

				lvi.SubItems.Add("第3列,第" + i + "行");

				this.listView1.Items.Add(lvi);
			}

			this.listView1.EndUpdate();  //结束数据处理，UI界面一次性绘制。

		}
		private void NewTabControl()
		{
			// 创建TabControl控件
			var tabControl = new TabControl();
			tabControl.Multiline = true; // 设置Multiline属性为True

			// 添加标签页
			var tabPage1 = new TabPage("标签页1");
			var tabPage2 = new TabPage("标签页2");
			var tabPage3 = new TabPage("标签页3");
			tabControl.TabPages.AddRange(new TabPage[] { tabPage1, tabPage2, tabPage3 });

			tabPage3.Controls.Add(new Label { Text = "这是标签页3的内容", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });

			// 添加TabControl控件到窗体
			this.Controls.Add(tabControl);
		}



		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBox1.SelectedItem != null)
			{
				MessageBox.Show($"选中项：{listBox1.SelectedItem}");

			}
		}

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void tabPage1_Click(object sender, EventArgs e)
		{

		}
	}
}
