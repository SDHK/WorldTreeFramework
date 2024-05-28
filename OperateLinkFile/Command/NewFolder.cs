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
	internal sealed class NewFolder
	{
		/// <summary>
		/// Command ID.
		/// </summary>
		public const int CommandId = 0x0200;

		/// <summary>
		/// Command menu group (command set GUID).
		/// </summary>
		public static readonly Guid CommandSet = new Guid("5304f298-039c-426f-a592-b3a2eb83dfe6");

		/// <summary>
		/// VS Package that provides this command, not null.
		/// </summary>
		private readonly AsyncPackage package;

		/// <summary>
		/// Initializes a new instance of the <see cref="NewFolder"/> class.
		/// Adds our command handlers for menu (commands must exist in the command table file)
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		/// <param name="commandService">Command service to add command to, not null.</param>
		private NewFolder(AsyncPackage package, OleMenuCommandService commandService)
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
		public static NewFolder Instance
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
			// Switch to the main thread - the call to AddCommand in NewFolder's constructor requires
			// the UI thread.
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

			OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
			Instance = new NewFolder(package, commandService);
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
			newFileWindow.Title = "请输入文件夹名称";
			newFileWindow.ShowDialog();
		}

		private void Execute(InputFieldControl inputFieldControl, bool bit)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			if (!bit) return;

			DTE2 dte = ServiceProvider.GetServiceAsync(typeof(DTE)).Result as DTE2;
			var selectedItem = dte.SelectedItems.Item(1);
			if (selectedItem == null || !(selectedItem.ProjectItem is ProjectItem projectItem)) return;

			//在路径创建文件并添加到项目中
			string folderPath = Path.Combine(projectItem.FileNames[0], inputFieldControl.TextBox.Text);

			//检测文件夹是否存在，路径是否正确
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}
			else
			{
				MessageBox.Show("文件夹已存在！！！");
			}

			//刷新项目配置
			string ProjectPath = projectItem.ContainingProject.FullName;
			RefreshProject(ProjectPath);
		}


		public void RefreshProject(string ProjectPath)
		{
			string text = File.ReadAllText(ProjectPath);
			text += " "; // 在文件尾部添加空格
			File.WriteAllText(ProjectPath, text); // 保存文件

			text.TrimEnd(); // 删除文件尾部空格
			File.WriteAllText(ProjectPath, text); // 保存文件
		}
	}
}
