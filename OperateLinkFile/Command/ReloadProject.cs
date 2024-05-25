using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using Task = System.Threading.Tasks.Task;

namespace OperateLinkFile.Command
{
	/// <summary>
	/// Command handler
	/// </summary>
	internal sealed class ReloadProject
	{
		/// <summary>
		/// Command ID.
		/// </summary>
		public const int CommandId = 0x0200;

		/// <summary>
		/// Command menu group (command set GUID).
		/// </summary>
		public static readonly Guid CommandSet = new Guid("7c59721e-b62a-45c9-a354-c6594c19e7ae");

		/// <summary>
		/// VS Package that provides this command, not null.
		/// </summary>
		private readonly AsyncPackage package;

		/// <summary>
		/// Initializes a new instance of the <see cref="ReloadProject"/> class.
		/// Adds our command handlers for menu (commands must exist in the command table file)
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		/// <param name="commandService">Command service to add command to, not null.</param>
		private ReloadProject(AsyncPackage package, OleMenuCommandService commandService)
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
		public static ReloadProject Instance
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
			// Switch to the main thread - the call to AddCommand in ReloadProject's constructor requires
			// the UI thread.
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

			OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
			Instance = new ReloadProject(package, commandService);
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
			if (dte == null) return;

			// 检查是否有未保存的更改
			if (dte.Documents.Cast<Document>().Any(doc => !doc.Saved))
			{
				// 如果有文件未保存成功,则弹出对话框让用户选择是否要重新加载
				var result = MessageBox.Show("有未保存的更改，是否要重新加载项目？", "警告", MessageBoxButtons.YesNo);
				if (result == DialogResult.No)
				{
					return;
				}
			}

			var selectedItems = dte.SelectedItems;
			//假如选择了多个项目，那么就遍历处理
			foreach (SelectedItem selectedItem in selectedItems)
			{
				if (selectedItem.Project != null)
				{
					//获取项目已打开的文件的路径
					List<string> openedFiles = GetOpenedFiles(selectedItem.Project);

					// 命令卸载项目
					dte.ExecuteCommand("Project.UnloadProject");
					// 命令加载项目
					dte.ExecuteCommand("Project.ReloadProject");


					//重新打开所有已打开的文件
					foreach (var file in openedFiles) dte.ItemOperations.OpenFile(file);
				}
			}
			return;
		}

		private List<string> GetOpenedFiles(Project project)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			List<string> openedFiles = new List<string>();

			foreach (ProjectItem item in project.ProjectItems)
			{
				// 检查项目项是否有文档和活动窗口
				if (item.Document != null && item.Document.ActiveWindow != null)
				{
					// 检查项目项是否有文件名
					openedFiles.Add(item.FileNames[0]);
				}
			}

			return openedFiles;
		}

	}
}
