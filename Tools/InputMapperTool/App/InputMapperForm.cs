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
			LoadFileSystemToTreeView();
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			TreeNode a = e.Node;
		}

		// 假设已有一个TreeView控件名为treeView1
		private void LoadFileSystemToTreeView()
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
	}
}
