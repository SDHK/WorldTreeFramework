using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Forms;
using Task = System.Threading.Tasks.Task;

namespace OperateLinkFile
{
	/// <summary>
	/// Command handler
	/// </summary>
	internal sealed class DeleteFile
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
		/// Initializes a new instance of the <see cref="DeleteFile"/> class.
		/// Adds our command handlers for menu (commands must exist in the command table file)
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		/// <param name="commandService">Command service to add command to, not null.</param>
		private DeleteFile(AsyncPackage package, OleMenuCommandService commandService)
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
		public static DeleteFile Instance
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
			// Switch to the main thread - the call to AddCommand in ReloadFile's constructor requires
			// the UI thread.
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

			OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
			Instance = new DeleteFile(package, commandService);
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
			DTE2 dte = ServiceProvider.GetServiceAsync(typeof(DTE)).Result as DTE2;

			var selectedItem = dte.SelectedItems.Item(1);
			if (selectedItem == null || !(selectedItem.ProjectItem is ProjectItem projectItem)) return;

			string filePath = projectItem.FileNames[0]; // 获取文件路径

			string oldFileName = Path.GetFileName(filePath);

			var result = MessageBox.Show($"确定删除文件：{oldFileName} \n{filePath}", "警告", MessageBoxButtons.YesNo);
			if (result == DialogResult.No) return;
			//检测这是文件夹还是文件
			if (Directory.Exists(filePath))
			{
				Directory.Delete(filePath, true);//删除文件夹
			}
			else if (File.Exists(filePath))
			{
				File.Delete(filePath);//删除文件
			}

		}
	}
}
