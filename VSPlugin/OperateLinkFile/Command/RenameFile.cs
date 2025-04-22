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
	internal sealed class RenameFile
	{
		/// <summary>
		/// Command ID.
		/// </summary>
		public const int CommandId = 0x0300;

		/// <summary>
		/// Command menu group (command set GUID).
		/// </summary>
		public static readonly Guid CommandSet = new Guid("f0caa44f-08f2-45f5-84ae-6e6ef252bdad");

		/// <summary>
		/// VS Package that provides this command, not null.
		/// </summary>
		private readonly AsyncPackage package;

		/// <summary>
		/// Initializes a new instance of the <see cref="RenameFile"/> class.
		/// Adds our command handlers for menu (commands must exist in the command table file)
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		/// <param name="commandService">Command service to add command to, not null.</param>
		private RenameFile(AsyncPackage package, OleMenuCommandService commandService)
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
		public static RenameFile Instance
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
			// Switch to the main thread - the call to AddCommand in RenameFile's constructor requires
			// the UI thread.
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

			OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
			Instance = new RenameFile(package, commandService);
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
			newFileWindow.Title = "重命名文件名称";
			newFileWindow.ShowDialog();
		}

		private void Execute(InputFieldControl inputFieldControl, bool bit)
		{
			if (!bit) return;

			DTE2 dte = ServiceProvider.GetServiceAsync(typeof(DTE)).Result as DTE2;
			var selectedItem = dte.SelectedItems.Item(1);
			if (selectedItem == null || !(selectedItem.ProjectItem is ProjectItem projectItem)) return;
			string filePath = projectItem.FileNames[0]; //获取文件路径
			if (!File.Exists(filePath)) return;

			filePath = Path.GetDirectoryName(filePath); //获取文件父级路径

			//在路径创建文件并添加到项目中
			string fileName = inputFieldControl.TextBox.Text;
			string newFile = Path.Combine(filePath, fileName);

			//检测文件是否存在，路径是否正确
			if (!File.Exists(newFile))
			{
				//重命名文件
				File.Move(projectItem.FileNames[0], newFile);
			}
			else
			{
				//提示文件重名
				MessageBox.Show("文件重名！！！");
			}
			dte.ItemOperations.OpenFile(newFile);

			//刷新项目配置
			CommandHelper.RefreshProject(projectItem.ContainingProject.FullName);
		}
	}
}
