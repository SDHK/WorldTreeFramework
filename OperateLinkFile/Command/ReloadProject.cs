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
				var result = MessageBox.Show("有未保存的更改，是否要重新加载？", "警告", MessageBoxButtons.YesNo);
				if (result == DialogResult.No)
				{
					return;
				}
			}
			//await VS.StatusBar.ShowProgressAsync("Step 3/3", 3, 3);
			var selectedItems = dte.SelectedItems;
			//假如选择了多个项目，那么就遍历处理
			foreach (SelectedItem selectedItem in selectedItems)
			{
				if (selectedItem.Project != null)
				{
					//获取项目已打开的文件的路径

					bool isUnity = IsUnityProject(selectedItem.Project);
					List<string> openedFiles = null;
					if (isUnity)
					{
						openedFiles = GetOpenedFiles(dte, selectedItem.Project.FullName);
					}
					else
					{
						openedFiles = GetOpenedFiles(dte, selectedItem.Project);
					}

					// 命令卸载项目
					dte.ExecuteCommand("Project.UnloadProject");
					// 命令加载项目
					dte.ExecuteCommand("Project.ReloadProject");

					//重新打开所有已打开的文件
					foreach (var file in openedFiles) dte.ItemOperations.OpenFile(file);
				}
			}
		}

		private List<string> GetOpenedFiles(DTE2 dte, Project project)
		{ 
			ThreadHelper.ThrowIfNotOnUIThread();
			List<string> openedFiles = new List<string>();
			Stack<ProjectItem> stack = new Stack<ProjectItem>();

			ProjectItem current;
			foreach (ProjectItem item in project.ProjectItems)
			{
				stack.Push(item);
				while (stack.Count > 0)
				{
					current = stack.Pop();

					// 检查项目项是否有文档和活动窗口
					if (current.Document != null && current.Document.ActiveWindow != null)
					{
						// 检查项目项是否有文件名
						string FileName = current.FileNames[0];
						openedFiles.Add(FileName);
					}

					// 将子项目项添加到栈中
					foreach (ProjectItem ProjectItem in current.ProjectItems)
					{
						stack.Push(ProjectItem);
					}
				}
			}
			return openedFiles;
		}

		private List<string> GetOpenedFiles(DTE2 dte, string projectPath)
		{
			//projectPath的父级目录
			projectPath = Path.GetDirectoryName(projectPath);

			List<string> openedFiles = new List<string>();

			// 获取项目目录中的所有C#文件
			string[] files = Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories);
			foreach (string file in files)
			{
				// 检查文件是否已在Visual Studio中打开
				Document document = dte.Documents.Cast<Document>().FirstOrDefault(doc => doc.FullName == file);
				if (document != null && document.ActiveWindow != null)
				{
					openedFiles.Add(file);
				}
			}
			return openedFiles;
		}

		private bool IsUnityProject(Project project)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			// 获取项目文件的路径
			string projectFilePath = Path.GetDirectoryName(project.FullName);
			//查找是否有Assets文件夹
			string[] dirs = Directory.GetDirectories(projectFilePath);
			foreach (string dir in dirs)
			{
				if (dir.EndsWith("Assets"))
				{
					return true;
				}
			}
			return false;
		}
	}
}
