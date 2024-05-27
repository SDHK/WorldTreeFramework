using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace OperateLinkFile
{
	/// <summary>
	/// Interaction logic for ToolWindowControl.
	/// </summary>
	public partial class ToolWindowControl : UserControl
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ToolWindowControl"/> class.
		/// </summary>
		public ToolWindowControl()
		{
			this.InitializeComponent();
		}

		public IAsyncServiceProvider ServiceProvider;

		/// <summary>
		/// Handles click on the button by displaying a message box.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event args.</param>
		[SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
		private void button_CreateFile(object sender, RoutedEventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			DTE2 dte = ServiceProvider.GetServiceAsync(typeof(DTE)).Result as DTE2;
			var selectedItem = dte.SelectedItems.Item(1);
			if (selectedItem == null || !(selectedItem.ProjectItem is ProjectItem projectItem)) return;
			string filePath = projectItem.FileNames[0]; //获取文件路径
			filePath = Path.GetDirectoryName(filePath); //获取文件父级路径

			//在路径创建文件并添加到项目中
			string FileName = TextBox.Text;
			string file = Path.Combine(filePath, FileName);


			TextBox.Text = "";
			//检测文件是否存在，路径是否正确
			if (!File.Exists(file))
			{
				//检测路径不存在创建父级文件夹
				string DirectoryPath = Path.GetDirectoryName(file);

				if (!Directory.Exists(DirectoryPath)) Directory.CreateDirectory(DirectoryPath);
				File.Create(file).Close();

				MessageBox.Show($"文件已创建：{file}");
			}
			else
			{
				MessageBox.Show("文件已存在！！！");
			}
		}

		private void button_RenameFile(object sender, RoutedEventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			DTE2 dte = ServiceProvider.GetServiceAsync(typeof(DTE)).Result as DTE2;
			var selectedItem = dte.SelectedItems.Item(1);
			if (selectedItem == null || !(selectedItem.ProjectItem is ProjectItem projectItem)) return;
			string filePath = projectItem.FileNames[0]; //获取文件路径
			string newFileName = TextBox.Text;

			TextBox.Text = "";
			//检测这是文件夹还是文件
			if (Directory.Exists(filePath))
			{
				//对文件夹名称修改
				string oldFileName = Path.GetFileName(filePath);
				string newFilePath = Path.Combine(Path.GetDirectoryName(filePath), newFileName);
				Directory.Move(filePath, newFilePath);
				MessageBox.Show($"文件夹：{oldFileName}\n重命名为:\n{newFilePath}");

			}
			else if (File.Exists(filePath))
			{
				//对文件名称修改
				string oldFileName = Path.GetFileName(filePath);
				string newFilePath = Path.Combine(Path.GetDirectoryName(filePath), newFileName);
				File.Move(filePath, newFilePath);
				MessageBox.Show($"文件：{oldFileName}\n重命名为:\n{newFilePath}");
			}
		}

	}
}