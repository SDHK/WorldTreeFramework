using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using OperateLinkFile.Windows;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Task = System.Threading.Tasks.Task;

namespace OperateLinkFile
{
	/// <summary>
	/// Command handler
	/// </summary>
	internal sealed class RenameFolder
	{
		/// <summary>
		/// Command ID.
		/// </summary>
		public const int CommandId = 0x0400;

		/// <summary>
		/// Command menu group (command set GUID).
		/// </summary>
		public static readonly Guid CommandSet = new Guid("5304f298-039c-426f-a592-b3a2eb83dfe6");

		/// <summary>
		/// VS Package that provides this command, not null.
		/// </summary>
		private readonly AsyncPackage package;

		/// <summary>
		/// Initializes a new instance of the <see cref="RenameFolder"/> class.
		/// Adds our command handlers for menu (commands must exist in the command table file)
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		/// <param name="commandService">Command service to add command to, not null.</param>
		private RenameFolder(AsyncPackage package, OleMenuCommandService commandService)
		{
			this.package = package ?? throw new ArgumentNullException(nameof(package));
			commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

			var menuCommandID = new CommandID(CommandSet, CommandId);
			var menuItem = new MenuCommand(this.Execute, menuCommandID);
			commandService.AddCommand(menuItem);
		}

		/// <summary>
		/// Gets the instance of the command.
		/// </summary>
		public static RenameFolder Instance
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the service provider from the owner package.
		/// </summary>
		private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
		{
			get
			{
				return this.package;
			}
		}

		/// <summary>
		/// Initializes the singleton instance of the command.
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		public static async Task InitializeAsync(AsyncPackage package)
		{
			// Switch to the main thread - the call to AddCommand in RenameFolder's constructor requires
			// the UI thread.
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

			OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
			Instance = new RenameFolder(package, commandService);
		}

		/// <summary>
		/// This function is the callback used to execute the command when the menu item is clicked.
		/// See the constructor to see how the menu item is associated with this function using
		/// OleMenuCommandService service and MenuCommand class.
		/// </summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		private void Execute(object sender, EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			InputFieldWindow newFileWindow = new InputFieldWindow();
			newFileWindow.callback = Execute;
			newFileWindow.Title = "重命名文件夹名称";
			newFileWindow.ShowDialog();
		}

		private void Execute(InputFieldControl inputFieldControl, bool bit)
		{
			if (!bit) return;

			DTE2 dte = ServiceProvider.GetServiceAsync(typeof(DTE)).Result as DTE2;
			var selectedItem = dte.SelectedItems.Item(1);
			if (selectedItem == null || !(selectedItem.ProjectItem is ProjectItem projectItem)) return;


			//获取链接文件夹的原始路径
			if (!(CommandHelper.GetOriginalPath(projectItem) is string folderPath))
			{
				// 拿不到则直接获取文件夹路径
				folderPath = projectItem.FileNames[0];
			}
			string folderDirectoryPath = Path.GetDirectoryName(folderPath); //获取文件夹父级路径

			if (!Directory.Exists(folderDirectoryPath)) return;

			//在路径创建文件夹并添加到项目中
			string newFolderPath = Path.Combine(folderDirectoryPath, inputFieldControl.TextBox.Text);

			//检测文件夹是否存在，路径是否正确
			if (!Directory.Exists(newFolderPath))
			{
				try
				{
					//重命名文件夹
					Directory.Move(folderPath, newFolderPath);
				}
				catch (IOException)
				{ 
					MessageBox.Show("文件夹被程序占用中！！！");
				}
			}
			else
			{
				//提示文件夹重名
				MessageBox.Show("文件夹重名！！！");
			}
		}
	}
}
