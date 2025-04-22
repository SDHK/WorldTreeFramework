using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Task = System.Threading.Tasks.Task;

namespace OperateLinkFile
{
	/// <summary>
	/// Command handler
	/// </summary>
	internal sealed class DeleteFolder
	{
		/// <summary>
		/// Command ID.
		/// </summary>
		public const int CommandId = 0x0300;

		/// <summary>
		/// Command menu group (command set GUID).
		/// </summary>
		public static readonly Guid CommandSet = new Guid("5304f298-039c-426f-a592-b3a2eb83dfe6");

		/// <summary>
		/// VS Package that provides this command, not null.
		/// </summary>
		private readonly AsyncPackage package;

		/// <summary>
		/// Initializes a new instance of the <see cref="DeleteFolder"/> class.
		/// Adds our command handlers for menu (commands must exist in the command table file)
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		/// <param name="commandService">Command service to add command to, not null.</param>
		private DeleteFolder(AsyncPackage package, OleMenuCommandService commandService)
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
		public static DeleteFolder Instance
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
			// Switch to the main thread - the call to AddCommand in DeleteFolder's constructor requires
			// the UI thread.
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

			OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
			Instance = new DeleteFolder(package, commandService);
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

			List<string> files = new List<string>();
			List<string> ProjectPaths = new List<string>();

			string ShowText = "";
			foreach (var selectedItem in dte.SelectedItems)
			{
				if (selectedItem == null || !(selectedItem is SelectedItem SelectedItem)) continue;
				var projectItem = SelectedItem.ProjectItem;
				if (projectItem == null) continue;

				//获取链接文件夹的原始路径
				if (!(CommandHelper.GetOriginalPath(projectItem) is string folderPath))
				{
					// 拿不到则直接获取文件夹路径
					folderPath = projectItem.FileNames[0];
				}

				if (!Directory.Exists(folderPath)) continue;//文件夹不存在

				folderPath = folderPath.TrimEnd('\\');
				ShowText += folderPath.Split('\\').Last() + "\n";
				files.Add(folderPath);
				string ProjectPath = projectItem.ContainingProject.FullName;
				if (!Directory.Exists(ProjectPath) || ProjectPaths.Contains(ProjectPath)) continue;
				ProjectPaths.Add(projectItem.ContainingProject.FullName);
			}

			var result = MessageBox.Show($"确定删除文件夹：\n{ShowText}这将会删除所有的子目录和文件！！！", "警告", MessageBoxButtons.YesNo);
			if (result == DialogResult.No) return;

			//删除文件
			foreach (var file in files) Directory.Delete(file, true);

			//刷新项目配置
			foreach (var ProjectPath in ProjectPaths) CommandHelper.RefreshProject(ProjectPath);
		}


	}
}
