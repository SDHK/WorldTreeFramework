using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Linq;
using Task = System.Threading.Tasks.Task;

namespace OperateLinkFile
{
	/// <summary>
	/// 重新加载解决方案
	/// </summary>
	internal sealed class ReloadSolution
	{
		/// <summary>
		/// Command ID.
		/// </summary>
		public const int CommandId = 0x0100;

		/// <summary>
		/// Command menu group (command set GUID).
		/// </summary>
		public static readonly Guid CommandSet = new Guid("290fe293-9565-4326-86d2-0b9e0b048177");

		/// <summary>
		/// VS Package that provides this command, not null.
		/// </summary>
		private readonly AsyncPackage package;

		/// <summary>
		/// Initializes a new instance of the <see cref="ReloadSolution"/> class.
		/// Adds our command handlers for menu (commands must exist in the command table file)
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		/// <param name="commandService">Command service to add command to, not null.</param>
		private ReloadSolution(AsyncPackage package, OleMenuCommandService commandService)
		{
			this.package = package ?? throw new ArgumentNullException(nameof(package));
			commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

			var menuCommandID = new CommandID(CommandSet, CommandId);
			var menuItem = new MenuCommand(this.ExecuteAsync, menuCommandID);
			commandService.AddCommand(menuItem);
		}

		/// <summary>
		/// Gets the instance of the command.
		/// </summary>
		public static ReloadSolution Instance
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
			// Switch to the main thread - the call to AddCommand in Command1's constructor requires
			// the UI thread.
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

			OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
			Instance = new ReloadSolution(package, commandService);
		}

		/// <summary>
		/// This function is the callback used to execute the command when the menu item is clicked.
		/// See the constructor to see how the menu item is associated with this function using
		/// OleMenuCommandService service and MenuCommand class.
		/// </summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		private void ExecuteAsync(object sender, EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			// 获取DTE服务
			var dte = ServiceProvider.GetServiceAsync(typeof(DTE)).Result as DTE2;

			// 获取当前解决方案路径
			string solutionPath = dte.Solution.FullName;

			// 检查是否有未保存的更改
			if (dte.Documents.Cast<Document>().Any(doc => !doc.Saved))
			{
				// 如果有文件未保存成功,则弹出对话框让用户选择是否要重新加载
				var result = System.Windows.Forms.MessageBox.Show("有未保存的更改，是否要重新加载？", "警告", MessageBoxButtons.YesNo);
				if (result == DialogResult.No)
				{
					return;
				}
			}

			// 关闭解决方案
			dte.Solution.Close();

			// 重新打开解决方案 
			dte.Solution.Open(solutionPath);
		}
	}
}
